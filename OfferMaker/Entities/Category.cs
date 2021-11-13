using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class Category : ICategory
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int ParentId { get; set; }

        public ObservableCollection<Category> Childs { get; set; } = new ObservableCollection<Category>();
    }
}
