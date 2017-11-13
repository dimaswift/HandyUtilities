using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeGenerator;
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;

namespace HandyUtilities
{
    public class AnimatorHashGenerator : Editor
    {
        [MenuItem("CONTEXT/Animator/Generate Hashes")]
        static void Generate(MenuCommand command)
        {
            if (!Directory.Exists(Application.dataPath + "/Scripts"))
                Directory.CreateDirectory(Application.dataPath + "/Scripts");
            var file = Application.dataPath + "/Scripts/AnimHash.cs";
            Class cls = null;
            Method initMethod = null;
            if (File.Exists(file))
            {
                cls = new ClassParser().Parse(File.ReadAllText(file));
                initMethod = cls.members.Find(m => m is Method) as Method;
                File.Delete(file);
            }
            else
            {
                cls = new Class("AnimHash", "public", "static");
                cls.AddDirective("UnityEngine");
                initMethod = new Method("void", "Init", "static", "public");
                initMethod.AddAttributes("RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)");
                cls.AddMember(initMethod);
            }
            var anim = command.context as Animator;
            foreach (var p in anim.parameters)
            {
                var name = Regex.Replace(p.name, @"\s+", "_");
                if (cls.members.Find(m => m.name == name) == null)
                {
                    cls.AddMember(new AutoProperty("int", name, "public", "private", "static"));
                    initMethod.AddLine(string.Format("{0} = Animator.StringToHash(\"{1}\");", name, p.name));
                    Debug.Log(string.Format("Hash for parameter \"{0}\" has been added to AnimHash class.", p.name));
                }
            }
            File.WriteAllText(file, cls.ToString());
            AssetDatabase.Refresh();
        }
    }
}
