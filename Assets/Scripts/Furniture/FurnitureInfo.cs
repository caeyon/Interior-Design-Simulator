[System.Serializable]
public class FurnitureInfo
{
    public string name;
    public int price;
    public string imageUrl;
    public string link;
    public string size;

    public FurnitureInfo(string name, int price, string imageUrl, string link, string size)
    {
        this.name = name;
        this.price = price;
        this.imageUrl = imageUrl;
        this.link = link;
        this.size = size;
    }
}
