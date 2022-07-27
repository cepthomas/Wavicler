
using System;
using System.Collections.Generic;


namespace MyUndo
{

    public interface IOperation
    {
        void Undo();

        void Redo();
    }



    // operations: 
    // ? change view - zoom H/V - just view
    // - cut/copy/paste - location - alter buffer or just view?
    // - amplitude adj - start/end/shape. general transforms? - alter buffer or just view?


    public class Cut : IOperation
    {
        public Cut(int start, int len)
        {

        }


        public void Undo()
        {

        }

        public void Redo()
        {

        }
    }

    public class OperationStack
    {
        /// <summary>The list of operations being managed.</summary>
        readonly List<IOperation> _ops = new();

        /// <summary>Where we are in _ops. This is actually one past the real index.</summary>
        int _index;

        /// <summary>TODO Disable temporarily while undoing an action.</summary>
        bool _gate = true;

        public bool CanUndo { get { return _ops.Count > 0; } }

        public bool CanRedo { get { return _ops.Count > _index - 1; } }

        public int UndoCount { get { return _index; } }

        public int RedoCount { get { return _ops.Count - _index; } }

        //TODO  bounds checking, limit size?


        public void Undo()
        {
            // undo decrements index, exec St[i].Undo()

            //AssertNoUndoGroupOpen();
            //if (undostack.Count > 0)
            //{
            //    IUndoableOperation uedit = (IUndoableOperation)undostack.Pop();
            //    redostack.Push(uedit);
            //    uedit.Undo();
            //    OnActionUndone();
            //}
        }

        public void Redo()
        {
            // redo exec St[i].Redo(), incr index.

            //AssertNoUndoGroupOpen();
            //if (redostack.Count > 0)
            //{
            //    IUndoableOperation uedit = (IUndoableOperation)redostack.Pop();
            //    undostack.Push(uedit);
            //    uedit.Redo();
            //    OnActionRedone();
            //}
        }

        public void Add(IOperation op)
        {
            // operations add to tail and incr index (just past last valid op)
            // Any other op besides redo removes to end then adds new op.

            //if (operation == null)
            //{
            //    throw new ArgumentNullException("operation");
            //}

            //if (AcceptChanges)
            //{
            //    StartUndoGroup();
            //    undostack.Push(operation);
            //    actionCountInUndoGroup++;
            //    if (TextEditorControl != null)
            //    {
            //        undostack.Push(new UndoableSetCaretPosition(this, TextEditorControl.ActiveTextAreaControl.Caret.Position));
            //        actionCountInUndoGroup++;
            //    }
            //    EndUndoGroup();
            //    ClearRedoStack();
            //}
        }

        public void ClearRedo()
        {
            //Remove to end.
        }

        /// <summary>
        /// Clears both the undo and redo stack.
        /// </summary>
        public void ClearAll()
        {
            _ops.Clear();
            _index = 0;
        }
    }
}
