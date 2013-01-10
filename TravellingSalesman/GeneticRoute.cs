using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravellingSalesman
{
    public class GeneticRoute : Route
    {
        public ushort[] Genes { get; private set; }

        public GeneticRoute(Graph graph, Random rand)
            : base(graph)
        {
            Genes = new ushort[graph.Count];
            for (int i = 0; i < Genes.Length; ++i)
                Genes[i] = (ushort) rand.Next(65536);

            UpdateIndicesFromGenes();
        }

        public GeneticRoute(Route route)
            : base(route)
        {
            Genes = new ushort[Graph.Count];

            UpdateGenesFromIndices();
        }

        public GeneticRoute(Graph graph, ushort[] genes)
            : base(graph)
        {
            if (genes.Length != graph.Count)
                throw new Exception("Incorrect gene count");

            Genes = genes;
            UpdateIndicesFromGenes();
        }

        public void UpdateIndicesFromGenes()
        {
            Clear();
            for (int i = 0; i < Graph.Count; ++i) {
                int k = i < Graph.Count - 1
                    ? Genes[i] % (Graph.Count - i)
                    : 0;

                int vIndex = this.SelectNextBest(k);
                base.Insert(vIndex, i);
            }
        }

        private void AddGene(int vIndex)
        {
            Genes[Count] = (ushort) (vIndex - Count);
        }

        public void UpdateGenesFromIndices()
        {
            int count = Count;
            Clear();
            while (Count < count) {
                AddGene(Count + FindStatistic(Count));
                base.Insert(Count, Count);
            }
            // Count count count Count
        }

        public override void Insert(int vIndex, int index)
        {
            if (vIndex < Count)
                throw new IndexOutOfRangeException();

            if (index != Count)
                throw new NotSupportedException();

            AddGene(index);

            base.Insert(vIndex, index);
        }

        public override void Swap(int i, int j)
        {
            throw new NotSupportedException();
        }

        public override void Splice(int i, int j)
        {
            throw new NotSupportedException();
        }

        public override void Reverse(int start, int count)
        {
            throw new NotSupportedException();
        }
    }
}
