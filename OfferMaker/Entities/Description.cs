using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared;

namespace OfferMaker
{
    public class Description : BaseEntity, IDescription
    {
        string text;
        bool isEnabled;
        bool isComment;

        public string Text
        {
            get => text;
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsComment
        {
            get => isComment;
            set
            {
                isComment = value;
                OnPropertyChanged();
            }
        }
    }
}
