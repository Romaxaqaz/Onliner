using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model.ProfileModel
{
    public class ProfileSearchModel
    {
        public string IdUser { get; set; }
        public string UrlImage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public string CommentsCount { get; set; }

        public ProfileSearchModel(string id, string urlImage,string name, string description, string status, string time, string commentsCount)
        {
            this.IdUser = id;
            this.UrlImage = urlImage;
            this.Name = name;
            this.Description = description;
            this.Status = status;
            this.Time = time;
            this.CommentsCount = commentsCount;
        }
    }
}
