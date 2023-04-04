﻿using SoftFluent.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyGrid.Abstractions
{
    public interface IPropertyGridEngine
    {
        IEnumerable Convert(object options);

    }
}
