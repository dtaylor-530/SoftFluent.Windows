using Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyGrid.Demo.Model
{
    public class ViewModel : IViewModel
    {
        public ViewModel()
        {
 
        }

        public Template Template { get; set; } 

        public Panel Panel { get; set; }

        public CollectionPanel CollectionPanel { get; set; }
    }


    public class Template
    {
        public string DataTemplateKey { get; set; }
    }

    public class CollectionPanel
    {
        public CollectionGrid Grid { get; set; }
    }

    public class Panel
    {
        public Grid Grid { get; set; }
        public string Type { get; set; } = "StackPanel";
    }


    public class Dock
    {
        //public Dock Row { get; set; }
    }

    public class Grid
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int RowSpan{ get; set; }
        public int ColumnSpan { get; set; }
    }


    public class CollectionGrid
    {
        public int Rows { get; set; }
        public int Columns { get; set; }

    }
}
