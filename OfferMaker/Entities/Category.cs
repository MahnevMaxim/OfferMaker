using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace OfferMaker
{
    public class Category : BaseEntity
    {
        string title;

        public int Id { get; set; }

        public string Title 
        { 
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }

        public int? ParentId { get; set; }

        public string Guid { get; set; }

        public string ParentGuid { get; set; }

        [JsonIgnore]
        public ObservableCollection<Nomenclature> Nomenclatures { get; set; } = new ObservableCollection<Nomenclature>();

        [JsonIgnore]
        public ObservableCollection<Category> Childs { get; set; } = new ObservableCollection<Category>();

        /// <summary>
        /// Закрываем конструктор.
        /// </summary>
        Category() { }

        public Category(string title) 
        {
            Title = title;
            Guid = System.Guid.NewGuid().ToString();
        }

        public override string ToString() => "Id:" + Id + " Title: " + Title+ " Guid: " + Guid;
    }
}
