using System.Collections.Generic;

namespace LaTeXGraphGen
{
    class LaTexElementSquence : LaTeXElement, IEnumerable<LaTeXElement>, ICollection<LaTeXElement>
    {
        private List<LaTeXElement> _elements;

        public LaTexElementSquence()
        {
            _elements = new List<LaTeXElement>();
        }

        public override void WriteLaTeX(System.IO.TextWriter writer)
        {
            foreach (var element in this) {
                element.WriteLaTeX(writer);
            }
        }
    
        public IEnumerator<LaTeXElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(LaTeXElement item)
        {
            _elements.Add(item);
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public bool Contains(LaTeXElement item)
        {
            return _elements.Contains(item);
        }

        public void CopyTo(LaTeXElement[] array, int arrayIndex)
        {
            _elements.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _elements.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(LaTeXElement item)
        {
            return _elements.Remove(item);
        }
    }
}
