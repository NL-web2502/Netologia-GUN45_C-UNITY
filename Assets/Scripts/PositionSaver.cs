using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}
		
		[SerializeField, ReadOnly]
		[Tooltip("Для заполнения этого поля воспользуйтесь контекстным меню в инспекторе и командой \"Create File\"")]
		private TextAsset _json;

		[field: SerializeField, HideInInspector]
		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода? 
			// Тогда выполнение пойдёт дальше, и JsonUtility.FromJsonOverwrite(_json.text, this) выбросит NullReferenceException, потому что _json равен null и обращения к .text не будет. return нужен, чтобы прервать Awake и не выполнять код, рассчитанный на существующий ассет.
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			var wrapper = new RecordsWrapper();
			JsonUtility.FromJsonOverwrite(_json.text, wrapper);
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)? 
			// JsonUtility.FromJsonOverwrite  перезаписывает поля объекта значениями из JSON. Если в JSON-файле (ассете) ещё нет данных о Records, то после FromJsonOverwrite поле останется null. Проверка позволяет избежать NullReferenceException при дальнейшем обращении к Records (например, в EditorMover.Start вызывается _save.Records.Clear(), а в OnDrawGizmos — Records.Count). Если null — создаём пустой список.
				Records = wrapper.items ?? new List<Data>(10);

		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			// OnDrawGizmos вызывается редактором очень часто (при перерисовке сцены), в том числе когда объект ещё не инициализирован или данных нет, а проверки позволяют просто ничего не рисовать, пока данных нет.
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента? 
			// Потому что data[0] уже обработан до цикла. В цикле берется curr и рисуется линия от prev к curr, а затем  prev = curr		
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке? 
			// Создаётся (или перезаписывается, если уже существует) файл Path.txt
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					return;
				}
			}
		}

		[Serializable]
		private class RecordsWrapper
		{
    		public List<Data> items;
		}

		private void OnDestroy()
		{
			if (_json == null) return;

   			var wrapper = new RecordsWrapper { items = Records ?? new List<Data>() };
    		var json = JsonUtility.ToJson(wrapper, true);

    		var relativePath = UnityEditor.AssetDatabase.GetAssetPath(_json);
    		if (string.IsNullOrEmpty(relativePath)) return;

    		var absolutePath = Path.Combine(
       			Application.dataPath.Replace("Assets", ""),
        		relativePath);

    File.WriteAllText(absolutePath, json);

    UnityEditor.EditorUtility.SetDirty(_json);
    UnityEditor.AssetDatabase.SaveAssets();
    UnityEditor.AssetDatabase.Refresh();
		}
#endif
	}
}