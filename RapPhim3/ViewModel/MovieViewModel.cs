namespace RapPhim3.ViewModel
{
    public class MovieViewModel
    {
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public int? Duration { get; set; }
        public string? LandscapeImage { get; set; }
        public string? PortraitImage { get; set; }
        public int? CountryId { get; set; }
        public string? TrailerUrl { get; set; }

        // Danh sách thể loại được chọn
        public List<int> GenreIds { get; set; } = new List<int>();

        // Chuỗi diễn viên (cách nhau bởi dấu phẩy)
        public string? Actors { get; set; }

        public string Directors { get; set; }
    }
}
