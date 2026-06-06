using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeacherCollection", menuName = "Collection/Teacher Collection", order = 2)]
public class TeacherCollection : ScriptableObject
{
    public List<TeacherData> list = new List<TeacherData>();
}
