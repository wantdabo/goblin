using YooAsset;

/// <summary>
/// ��Դ�ļ���ѯ������
/// </summary>
public class GameQueryServices : IBuildinQueryServices
{
    /// <summary>
    /// ��ѯ�����ļ���ʱ���Ƿ�ȶ��ļ���ϣֵ
    /// </summary>
    public static bool CompareFileCRC = false;

    public bool Query(string packageName, string fileName, string fileCRC)
    {
        // ע�⣺fileName�����ļ���ʽ
        return StreamingAssetsHelper.FileExists(packageName, fileName, fileCRC);
    }
}