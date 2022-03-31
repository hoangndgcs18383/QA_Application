namespace QA_Application.Models
{
    public class Summernote
    {
        public Summernote(string iDEditor, bool loadLibrary = true)
        {
            IDEditor = iDEditor;
            LoadLibrary = loadLibrary;
        }
        public string IDEditor { get; set; }
        public bool LoadLibrary { get; set; }

        public int height { get; set; } = 120;
        public string toolbar { get; set; } = @"
        [
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontsize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['height', ['height']]
        ]
        ";
    }
}
