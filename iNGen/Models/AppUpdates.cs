namespace iNGen.Models
{
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AppUpdates
    {

        private AppUpdatesAppUpdate[] appUpdateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AppUpdate")]
        public AppUpdatesAppUpdate[] AppUpdate
        {
            get
            {
                return this.appUpdateField;
            }
            set
            {
                this.appUpdateField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AppUpdatesAppUpdate
    {

        private decimal versionField;

        private string changeNotesField;

        /// <remarks/>
        public decimal Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        public string ChangeNotes
        {
            get
            {
                return this.changeNotesField;
            }
            set
            {
                this.changeNotesField = value;
            }
        }
    }

}