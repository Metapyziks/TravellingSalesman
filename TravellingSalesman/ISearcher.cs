﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesman
{
    public interface ISearcher
    {
        Route Search( Graph graph, bool printProgress = false );
    }
}
