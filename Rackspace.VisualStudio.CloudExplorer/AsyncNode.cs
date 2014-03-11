namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using DialogResult = System.Windows.Forms.DialogResult;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

    public abstract class AsyncNode : Node
    {
        private volatile Task _updateTask;
        private volatile Task _deleteTask;
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

        public bool IsDeleting
        {
            get
            {
                return _deleteTask != null;
            }
        }

        public override sealed string Label
        {
            get
            {
                string displayText = DisplayText;
                if (IsRefreshing)
                    return string.Format("{0} (Refreshing...)", displayText);
                if (IsDeleting)
                    return string.Format("{0} (Deleting...)", displayText);

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

        public override int CompareUnique(Node node)
        {
            int result = string.Compare(Label, node.Label, StringComparison.OrdinalIgnoreCase);
            if (result != 0)
                return result;

            return string.Compare(Label, node.Label, StringComparison.Ordinal);
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

        public override sealed bool ConfirmDeletingNode()
        {
            DialogResult dialogResult = ConfirmUserDeletingNodeImpl();
            switch (dialogResult)
            {
            case DialogResult.OK:
            case DialogResult.Yes:
                lock (this)
                {
                    Task deleteTask = _deleteTask;
                    if (deleteTask == null)
                    {
                        Task<bool> nodeDeleteTask = DeleteNodeAsync(CancellationToken.None);
                        if (nodeDeleteTask.IsCompleted)
                            return nodeDeleteTask.Result;
                        else
                            _deleteTask = nodeDeleteTask.ContinueWith(UpdateDisplayAfterDeleted);
                    }
                }

                INodeSite nodeSite = GetNodeSite();
                if (nodeSite != null)
                    nodeSite.UpdateLabel();

                return false;

            default:
                break;
            }

            return false;
        }

        protected virtual DialogResult ConfirmUserDeletingNodeImpl()
        {
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            string message = "Are you sure you want to delete the item?";
            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

        protected void TryUpdateLabel()
        {
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite != null)
                nodeSite.UpdateLabel();
        }

        protected void UpdateDisplayAfterDeleted(Task<bool> deleteTask)
        {
            if (deleteTask.IsFaulted || deleteTask.IsCanceled)
            {
                Exception ignored = deleteTask.Exception;
                _deleteTask = null;
                TryUpdateLabel();
                return;
            }

            if (!deleteTask.IsCompleted || !deleteTask.Result)
                return;

            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return;

            if (_deleteTask != null)
            {
                nodeSite.Remove();
                _deleteTask = null;
            }
        }

        protected abstract Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken);

        protected virtual Task<bool> DeleteNodeAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
