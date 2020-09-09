using System.Linq;
using System.Reflection;

namespace CSBEF.Models
{
    /// <summary>
    /// TODO: To be translated into English
    /// Modül kütüphanelerini içeri aktarırken kullanılan tanımlama modelidir.
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// TODO: To be translated into English
        /// Modülün ismidir.
        /// Örnek: CSBEF.Module.UserManagement
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Modülün içeri aktarılmış kütüphanesidir (dll)
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// TODO: To be translated into English
        /// Modülün kısa adıdır.
        /// Örnek: "CSBEF.Module.UserManagement" için => "UserManagement"
        /// </summary>
        public string ShortName
        {
            get
            {
                return Name.Split('.').Last();
            }
        }

        /// <summary>
        /// TODO: To be translated into English
        /// Modülün kütüphane dosyası olan "dll" dosyasının bulunduğu dizinin yoludur.
        /// </summary>
        public string Path { get; set; }
    }
}