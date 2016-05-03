namespace Onliner.Model.ProfileModel
{
    public class ProfileSearchModel : IProfileSearch
    {
        public string IdUser { get; set; }
        public string UrlImage { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Time { get; set; }
        public string CommentsCount { get; set; }

        public ProfileSearchModel(string id, string urlImage, string name, string description, string status, string time, string commentsCount)
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
