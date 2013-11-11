namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;

    public abstract class AsyncNode : Node
    {
        private volatile Task _updateTask;
        private Node[] _children;

        public AsyncNode()
        {
        }

        public bool IsRefreshing
        {
            get
            {
                return _updateTask != null;
            }
        }

        public override sealed string Label
        {
            get
            {
                string displayText = DisplayText;
                if (IsRefreshing)
                    return string.Format("{0} (Refreshing...)", displayText);

                return displayText;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        protected abstract string DisplayText
        {
            get;
        }

        public override sealed bool CanEditLabel()
        {
            return false;
        }

        public override sealed Node[] CreateChildren()
        {
            lock (this)
            {
                Task updateTask = _updateTask;
                if (updateTask == null)
                {
                    Task<Node[]> nodeUpdateTask = CreateChildrenAsync(CancellationToken.None);
                    if (nodeUpdateTask.IsCompleted)
                        nodeUpdateTask.ContinueWith(RefreshNodeDisplay, TaskContinuationOptions.ExecuteSynchronously);
                    else
                        _updateTask = nodeUpdateTask.ContinueWith(RefreshNodeDisplay);
                }
            }

            return _children ?? new Node[0];
        }

        protected void RefreshNodeDisplay(Task<Node[]> childrenTask)
        {
            Node[] oldChildren = Interlocked.Exchange(ref _children, childrenTask.Result);

            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return;

            if (_updateTask != null)
            {
                nodeSite.ResetChildren();
                foreach (var child in _children)
                    nodeSite.AddChild(child);

                _updateTask = null;
                nodeSite.UpdateLabel();
            }
        }

        protected abstract Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken);
    }
}
