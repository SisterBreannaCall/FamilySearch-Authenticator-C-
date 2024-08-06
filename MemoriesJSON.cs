using Newtonsoft.Json;

namespace MemoriesResource
{
    public class Artifact
    {
        public string href;
    }

    public class ArtifactMetadatum
    {
        public string filename;
        public int width;
        public int heigh;
        public int size;
        public string screeningState;
        public string displayState;
        public List<Qualifier> qualifiers;
    }

    public class Attribution
    {
        public Contributor contributor;
    }

    public class Comments
    {
        public string href;
    }

    public class Contributor
    {
        public string resource;
        public string resourceId;
    }

    public class Coverage
    {
        public string href;
    }

    public class Image
    {
        public string href;
    }

    public class ImageDeepZoomLite
    {
        public string href;
    }

    public class ImageIcon
    {
        public string href;
    }

    public class ImageThumbnail
    {
        public string href;
    }

    public class Last
    {
        public string href;
    }

    public class Links
    {
        public Next next;
        public Last last;
        public Person person;
        public Self self;

        [JsonProperty("image-icon")]
        public ImageIcon imageicon;
        public Artifact artifact;
        public Coverage coverage;
        public Image image;
        public Persons persons;

        [JsonProperty("image-deep-zoom-lite")]
        public ImageDeepZoomLite imagedeepzoomlite;
        public Comments comments;
        public Memory memory;

        [JsonProperty("image-thumbnail")]
        public ImageThumbnail imagethumbnail;
    }

    public class Memory
    {
        public string href;
    }

    public class Next
    {
        public string href;
    }

    public class Person
    {
        public string href;
    }

    public class Persons
    {
        public string href;
    }

    public class Qualifier
    {
        public string name;
    }

    public class MemoriesJson
    {
        public Links links;
        public List<object> persons;
        public List<SourceDescription> sourceDescriptions;
    }

    public class Self
    {
        public string href;
    }

    public class SourceDescription
    {
        public string id;
        public string mediaType;
        public string about;
        public Attribution attribution;
        public string resourceType;
        public object created;
        public Links links;
        public List<Title> titles;
        public List<ArtifactMetadatum> artifactMetadata;
    }

    public class Title
    {
        public string value;
    }
}
