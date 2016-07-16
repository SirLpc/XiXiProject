using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// 自定义projects面板的右键单击菜单项，搜索使用到这个脚本的预制体。
/// 使用简单地在项目视图中选择 MonoBehaviour 资产并 右键单击它，
/// 选择选项"Find References in Prefabs"。如果prefabs 被发现，它们将会选择!
/// todo 没有排除任何路径
/// </summary>
public class FindPrefabsWithScript
{
    #region Const Members

    /// <summary>
    /// 我们查找 .prefab 文件的搜索模式
    /// </summary>
    private const string PREFAB_FILE_SEARCH_PATTERN = "t:Prefab";

    /// <summary>
    /// 搜索进度栏标题的格式字符串
    /// </summary>
    private const string PROGRESS_BAR_TITLE_FORMAT = "Searching for any prefabs with a {0} Component!";

    /// <summary>
    /// 搜索进度条消息的格式字符串
    /// </summary>
    private const string PROGRESS_BAR_MESSAGE_FORMAT = "Searching prefab ({0}/{1})";

    /// <summary>
    /// 当我们得到一个不太可能错误的格式字符串
    /// </summary>
    private const string ERROR_MESSAGE_FORMAT =
        "{0} encountered an unexptected error! Could not parse the System.Type from the script asset!";

    /// <summary>
    /// 我们并没有找到任何预置有指定的组件显示格式字符串
    /// </summary>
    private const string NOT_FOUND_MESSAGE_FORMAT = "Did not find any prefabs which had a {0} Component attached!";

    /// <summary>
    /// 我们发现结果和想要向用户显示的计数，我们将使用格式字符串
    /// </summary>
    private const string SUCCESS_MESSAGE_FORMAT = "Found {0} prefabs with an attached -{1}- Component!";

    #endregion Const Members

    #region Methods

    [MenuItem("Assets/Find MonoScripe Ref in Prefabs", false, 0)]
    private static void Find()
    {
        //在测试之后似乎撤消自动记录Selection更改。
        //所以我不手动注册任何撤消，因为这真的不修改 Selection.activeIds。
        //得到 Selection.activeObject 的 System.Type。
        var selectedType = GetSelectedType();

        // 安全判断 永远都要有偶！
        if (selectedType == null)
        {
            Debug.LogErrorFormat(ERROR_MESSAGE_FORMAT, typeof(FindPrefabsWithScript).Name);
            return;
        }

        // 获取project中的每个预设的路径
        var allMatches = GetPrefabsWhoHaveScript(selectedType).ToArray();

        if (allMatches == null || allMatches.Length == 0)
        {
            DisplayNoResultsFoundMessage(selectedType);
            return;
        }

        // 向用户显示结果
        HighlightResults(allMatches, selectedType);

    }

    /// <summary>
    /// 我们验证功能，将只允许用户选择菜单项，如果选择了 MonoScript
    /// </summary>
    [MenuItem("Assets/Find MonoScripe Ref in Prefabs", true)]
    private static bool IsValidScriptSelected()
    {
        //我们只知道如何找到 MonoBehaviours。选定的脚本是不是从 MonoBehaviour 派生
        return IsMonoBehaviour(GetSelectedType());
    }

    /// <summary>
    /// 检查当前选定的 activeObject。 返回所选脚本的类型名称
    /// </summary>
    /// <returns></returns>
    private static System.Type GetSelectedType()
    {
        //当前所选内容转换为一个MonoScript
        var currentSelection = Selection.activeObject as MonoScript;

        //我们的 currentSelection 不能为空，我们的 IsScriptSelected 函数应
        //防止Unity使我们能够在非 MonoScript 对象上运行

        if (currentSelection == null)
        {
            //如果转换失败，然后reutrn null
            return null;
        }

        //实际上所选对象的脚本。 返回该类的名称
        return currentSelection.GetClass();
    }

    /// <summary>
    /// 只是因为选择了一个脚本文件，这并不意味着该脚本本身是 MonoBehaviour，它可能是很多其他的东西。我们只可以搜索为从 MonoBehavior 派生的脚本的预置
    /// </summary>
    private static bool IsMonoBehaviour(System.Type selectedType)
    {
        //Sanity
        if (selectedType == null)
        {
            return false;
        }

        //确保selectedType来自MonoBehaviour
        return typeof(MonoBehaviour).IsAssignableFrom(selectedType);
    }

    /// <summary>
    /// Log 到控制台，我们没有找到任何结果。
    /// </summary>
    private static void DisplayNoResultsFoundMessage(System.Type selectedType)
    {
        Debug.LogFormat(NOT_FOUND_MESSAGE_FORMAT, selectedType.Name);
    }

    /// <summary>
    /// 搜索项目中的所有预设 并返回具有指定脚本附加的任何。
    /// </summary>
    /// <param name="selectedType">The System.Type to search for (must derive from MonoBehaviour)</param>
    /// <returns>具有附加脚本组件的每个预置的对象id</returns>
    private static IEnumerable<int> GetPrefabsWhoHaveScript(System.Type selectedType)
    {
        //项目中的所有预设
        var allPrefabs = GetPathToEachPrefabInProject();

        float totalPrefabCount = allPrefabs.Count();
        float currentPrefab = 1;

        //迭代项目中的所有预设的
        foreach (var prefabPath in allPrefabs)
        {
            //搜索可能需要很长时间在一个庞大的工程，向用户显示一个可取消进度栏，以防他们感到厌倦。
            if (EditorUtility.DisplayCancelableProgressBar(string.Format(PROGRESS_BAR_TITLE_FORMAT, selectedType.Name),
                string.Format(PROGRESS_BAR_MESSAGE_FORMAT, currentPrefab, totalPrefabCount),
                currentPrefab / totalPrefabCount))
            {
                //用户取消了！ Clear的进度栏和返回空！
                EditorUtility.ClearProgressBar();
                yield break;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) as GameObject;

            //看预设是否存在该组件
            if (PrefabHasScript(prefab, selectedType))
            {
                //确实！ 返回此预置的实例id
                yield return prefab.GetInstanceID();
            }

            // 增量计数函数，所以进度条显示正确。
            currentPrefab++;
        }

        //在我们成功地完成整个搜索后，确保我们Clear进度栏。
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 返回每个 AssetDatabase 路径。 在项目中的prefab。
    /// </summary>
    private static IEnumerable<string> GetPathToEachPrefabInProject()
    {
        foreach (var prefabGUID in AssetDatabase.FindAssets(PREFAB_FILE_SEARCH_PATTERN))
        {
            yield return AssetDatabase.GUIDToAssetPath(prefabGUID);
        }
    }

    /// <summary>
    /// 搜索指定的预设 层次结构，看看它是否已附加的selectedType。
    /// </summary>
    public static bool PrefabHasScript(GameObject prefab, System.Type selectedType)
    {
        //Sanity
        if (prefab == null)
        {
            return false;
        }

        // 现在，加载预置我们可以看到是否有附加我们脚本组件。首先搜索根上而不是搜索中的孩子，这可以提供有点大层次结构的加速。
        var allComponents = prefab.GetComponents(selectedType);

        if (allComponents.Length != 0)
        {
            return true;
        }
        else
        {
            //我们找不到组件的根上，子对象上搜索.。
            allComponents = prefab.GetComponentsInChildren(selectedType, true);

            if (allComponents != null && allComponents.Length > 0)
            {
                //有一个组件，返回根目录的id。
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 使Project视图 显示已更新的选择。
    /// </summary>
    private static void HighlightResults(int[] ids, System.Type selectedType)
    {
        // 更新selection，所以如果需要，用户可以将其拖动到场景中。
        Selection.instanceIDs = ids;
        // 试着在摆弄通过反射内部调用，遗憾的是：结果并不一致。直到unity公开一种显示在project视图中的进行多选方式，这是我们所能的做:
        EditorGUIUtility.PingObject(Selection.instanceIDs.Last());

        Debug.LogFormat(SUCCESS_MESSAGE_FORMAT, ids.Length, selectedType.Name);

        // 写入文件，弹出文件
        string path = Application.dataPath + "/FindPrefabWithScriptResoult.txt";
        File.Delete(path);
        using (FileStream fs = File.Create(path))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(string.Format(SUCCESS_MESSAGE_FORMAT, ids.Length, selectedType.Name));
                for (int n = 0; n < ids.Length; n++)
                {
                    string str = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(ids[n]));
                    sw.WriteLine(string.Format("      -> {0}、{1}",n, str));
                    Debug.Log("    " + str);
                }
            }
        }
        // 弹出文件！
        System.Diagnostics.Process.Start("notepad.exe", path);
    }

    #endregion Methods
}