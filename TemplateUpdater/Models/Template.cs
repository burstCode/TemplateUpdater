namespace TemplateUpdater.Models
{
    public class Template
    {
        public int Id { get; set; }
        public string TemplateFilename { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Version { get; set; }
        public byte[] Content { get; set; }
    }
}
