[System.Serializable]
public class FurnitureData
{
    public string furnitureName; //���� �̸� (�ĺ� �뵵)
    public string originalName;
    public float collideCm; //�浹 ����
    public string siteName;
    public string size;

    public FurnitureData(string name, string originalName, string siteName, string size)
    {
        furnitureName = name;
        this.originalName = originalName;
        this.siteName = siteName;
        this.size = size;
    }

    public void setCollideCm(float collideCm)
    {
        this.collideCm = collideCm;
    }

    public void setsiteName(string siteName)
    {
        this.siteName = siteName;
    }

    public void setSize(string size)
    {
        this.size = size;
    }
}
