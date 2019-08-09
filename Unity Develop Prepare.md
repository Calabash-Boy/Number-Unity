## Unity Develop Prepare
Splash-->main-->game

###如何制作Splash页:

打开Edit-Project Setting-Player:
![splash设置](/Users/xianqiang.guo/Desktop/Work/Unity/splash_setting.png)
在这里可以设置AppIcon,启动过程中需要显示的Logo;  
个人版的无法去除Unity自带的Logo和显示;
要达到竞品的展示效果,只需要设置两个Logo显示就可以了;  
支持StoryBoard以及自定义启动图,在Unity中无法预览,只有导出到Xcode工程中才可以看到效果;

--

1. 粒子动画
2. 场景切换
3. 计时器的实现
4. 对象池

###字体导入,多语言
找到对应的字体文件,直接拖入到Assert/Font文件夹,在设置字体的时候就可以选择;

- 那么如何自己制作字体呢?  
虽然可以方便的导入字体来使用,但是独特风格的数字和字母还是需要自定义的,这里可以使用Unity的CustomFont来实现,基于提供的一组数字图片或者字母图片来实现:
[制作自定义字体](https://blog.csdn.net/qq_28849871/article/details/77719054)  
**提供的图片中数字或者字母必须是白色的,底部透明的png图片.**

- 如何设置多语言?  
很多游戏有自己的多语言,在iOS开发中,通过本地化同一个key值对应不同的语言版本来实现,每种语言都是在一个Bundle中,通过切换Bundle来找到不同的语言;  
在Unity中,实现原理是一样的,但是在可视化界面上是没有Bundle这种类似的概念的,需要自己创建text文件,然后写一个本地化管理类的脚本把所有的keyValue放到字典中,对外提供一个根据key或者对应语言的API;  

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    //单例模式  
    private static LocalizationManager _instance;

    public static LocalizationManager GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocalizationManager();
            }

            return _instance;
        }
    }

    private const string chinese = "Chinese";
    private const string english = "English";

    //选择自已需要的本地语言  
    public const string language = chinese;


    private Dictionary<string, string> dic = new Dictionary<string, string>();
    /// <summary>  
    /// 读取配置文件，将文件信息保存到字典里  
    /// </summary>  
    public LocalizationManager()
    {
        TextAsset ta = Resources.Load<TextAsset>(language);
        string text = ta.text;

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (line == null)
            {
                continue;
            }
            string[] keyAndValue = line.Split('=');
            dic.Add(keyAndValue[0], keyAndValue[1]);
        }
    }

    /// <summary>  
    /// 获取value  
    /// </summary>  
    /// <param name="key"></param>  
    /// <returns></returns>  
    public string GetValue(string key)
    {
        if (dic.ContainsKey(key) == false)
        {
            return null;
        }
        string value = null;
        dic.TryGetValue(key, out value);
        return value;
    }  
}

```

再写一个脚本,有一个公开变量key,内部实现是根据这个key调用本地化的脚本的API,把这个脚本绑定到Text组件上即可; 

```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationText : MonoBehaviour
{
    public string key = " ";
    private void Start()
    {
        GetComponent<Text>().text = LocalizationManager.GetInstance.GetValue(key);
    }  
}



```
 
对于已经销毁的Text,在Awake中调用赋值.text的方法,对于还存在的Text组件,只能手动去刷新了;C#应该也有发送通知的功能;

对于现在组内采取的本地化方案,感觉还有可改进的余地;

--

###本地存储 
Unity中采用的是PlayerPrefab这个类才实现本地存储,从这个类开放的API可以看出这个类,只能存储Int,Float,String类型的数据;  
下面是一个扩展的类,对数组和其他类型做了一些扩展;

```
using System;
using UnityEngine;

/// <summary>
/// 本地数据管理类 
/// </summary>
public class GameDataManager
{

    /// <summary>
    /// 储存Bool
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetString(key + "Bool", value.ToString());
    }

    /// <summary>
    /// 取Bool
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static bool GetBool(string key)
    {
        try
        {
            return bool.Parse(PlayerPrefs.GetString(key + "Bool"));
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }

    }


    /// <summary>
    /// 储存String
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    /// <summary>
    /// 取String
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    /// <summary>
    /// 储存Float
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    /// <summary>
    /// 取Float
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static float GetFloat(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    /// <summary>
    /// 储存Int
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }


    /// <summary>
    /// 取Int
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static int GetInt(string key)
    {
        return PlayerPrefs.GetInt(key);
    }



    /// <summary>
    /// 储存IntArray
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetIntArray(string key, int[] value)
    {

        for (int i = 0; i < value.Length; i++)
        {
            PlayerPrefs.SetInt(key + "IntArray" + i, value[i]);
        }
        PlayerPrefs.SetInt(key + "IntArray", value.Length);
    }

    /// <summary>
    /// 取IntArray
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static int[] GetIntArray(string key)
    {
        int[] intArr = new int[1];
        if (PlayerPrefs.GetInt(key + "IntArray") != 0)
        {
            intArr = new int[PlayerPrefs.GetInt(key + "IntArray")];
            for (int i = 0; i < intArr.Length; i++)
            {
                intArr[i] = PlayerPrefs.GetInt(key + "IntArray" + i);
            }
        }
        return intArr;
    }

    /// <summary>
    /// 储存FloatArray
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetFloatArray(string key, float[] value)
    {

        for (int i = 0; i < value.Length; i++)
        {
            PlayerPrefs.SetFloat(key + "FloatArray" + i, value[i]);
        }
        PlayerPrefs.SetInt(key + "FloatArray", value.Length);
    }

    /// <summary>
    /// 取FloatArray
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static float[] GetFloatArray(string key)
    {
        float[] floatArr = new float[1];
        if (PlayerPrefs.GetInt(key + "FloatArray") != 0)
        {
            floatArr = new float[PlayerPrefs.GetInt(key + "FloatArray")];
            for (int i = 0; i < floatArr.Length; i++)
            {
                floatArr[i] = PlayerPrefs.GetFloat(key + "FloatArray" + i);
            }
        }
        return floatArr;
    }


    /// <summary>
    /// 储存StringArray
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetStringArray(string key, string[] value)
    {

        for (int i = 0; i < value.Length; i++)
        {
            PlayerPrefs.SetString(key + "StringArray" + i, value[i]);
        }
        PlayerPrefs.SetInt(key + "StringArray", value.Length);
    }

    /// <summary>
    /// 取StringArray
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static string[] GetStringArray(string key)
    {
        string[] stringArr = new string[1];
        if (PlayerPrefs.GetInt(key + "StringArray") != 0)
        {
            stringArr = new string[PlayerPrefs.GetInt(key + "StringArray")];
            for (int i = 0; i < stringArr.Length; i++)
            {
                stringArr[i] = PlayerPrefs.GetString(key + "StringArray" + i);
            }
        }
        return stringArr;
    }


    /// <summary>
    /// 储存Vector2
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetVector2(string key, Vector2 value)
    {
        PlayerPrefs.SetFloat(key + "Vector2X", value.x);
        PlayerPrefs.SetFloat(key + "Vector2Y", value.y);

    }

    /// <summary>
    /// 取Vector2
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static Vector2 GetVector2(string key)
    {
        Vector2 Vector2;
        Vector2.x = PlayerPrefs.GetFloat(key + "Vector2X");
        Vector2.y = PlayerPrefs.GetFloat(key + "Vector2Y");
        return Vector2;
    }


    /// <summary>
    /// 储存Vector3
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetVector3(string key, Vector3 value)
    {
        PlayerPrefs.SetFloat(key + "Vector3X", value.x);
        PlayerPrefs.SetFloat(key + "Vector3Y", value.y);
        PlayerPrefs.SetFloat(key + "Vector3Z", value.z);
    }

    /// <summary>
    /// 取Vector3
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static Vector3 GetVector3(string key)
    {
        Vector3 vector3;
        vector3.x = PlayerPrefs.GetFloat(key + "Vector3X");
        vector3.y = PlayerPrefs.GetFloat(key + "Vector3Y");
        vector3.z = PlayerPrefs.GetFloat(key + "Vector3Z");
        return vector3;
    }


    /// <summary>
    /// 储存Quaternion
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public static void SetQuaternion(string key, Quaternion value)
    {
        PlayerPrefs.SetFloat(key + "QuaternionX", value.x);
        PlayerPrefs.SetFloat(key + "QuaternionY", value.y);
        PlayerPrefs.SetFloat(key + "QuaternionZ", value.z);
        PlayerPrefs.SetFloat(key + "QuaternionW", value.w);
    }

    /// <summary>
    /// 取Quaternion
    /// </summary>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static Quaternion GetQuaternion(string key)
    {
        Quaternion quaternion;
        quaternion.x = PlayerPrefs.GetFloat(key + "QuaternionX");
        quaternion.y = PlayerPrefs.GetFloat(key + "QuaternionY");
        quaternion.z = PlayerPrefs.GetFloat(key + "QuaternionZ");
        quaternion.w = PlayerPrefs.GetFloat(key + "QuaternionW");
        return quaternion;
    }
}

```

这个类可以来存储一些简单的数据,如果需要数据库存储的话,就需要使用Sqlite来管理了;  
[Unity中使用Sqlite](https://blog.csdn.net/xic_xxx/article/details/75145320)  
读取,修改的方法都需要自己来封装,试着找了下轮子:  

--

###Audio的导入
使用AudioSource组件来播放音频;可以把Source当做播放器,而AudioClip是磁带,磁带是可以切换的;
移动端目前只支持mp3格式;  
关于资源文件在Unity中的加载需要另外学习,因此使用最简单的加载方式,把音频放在了Resources文件夹下,这样就可以使用系统提供的最简单的加载API;

```
//播放音效
        AudioSource source = GetComponent<AudioSource>();  
        AudioClip newClip = Resources.Load<AudioClip>("Music/enter_game");
        source.clip = newClip;
        source.Play();
```
前提是目录如下:

![](/Users/xianqiang.guo/Pictures/Snip20190808_3.png)

声音的知识还有混音器(OutPut)的使用,按下不表;


--
8. iTween落下效果
9. 骨骼动画