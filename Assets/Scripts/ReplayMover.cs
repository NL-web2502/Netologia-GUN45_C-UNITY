using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
			////todo comment: зачем нужны эти проверки?
			/// есть ли на обьекте компонентPositionSaver. Есть ли записанные точки
			if (!TryGetComponent(out _save) || _save.Records.Count == 0) 
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				// чтобы Update перестал вызываться
				enabled = false;
			}
		}

		private void Update()
		{
			if (_save == null || _save.Records == null || _index >= _save.Records.Count)
    		{
       			 enabled = false;
       			 return;
    		}
	
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)?
			//  Сравнивает текущее игровое время с временем текущей (следующей) записанной точки.
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				// Это защита от выхода за границы списка. Когда след точки нет, воспроизведение заканчивается, компонент выключается
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
					return; 
				}

				curr = _save.Records[_index];
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			//Это вычисление нормализованного параметра интерполяции. Time.time - _prev.Time — сколько времени прошло с момента предыдущей точки. curr.Time - _prev.Time — длительность всего сегмента между точками. В итоге показывается насколько мы "продвинулись" по сегменту
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
			//todo comment: Зачем нужна эта проверка?
			//Если curr.Time == _prev.Time (две точки записаны в один и тот же момент времени, что возможно при быстрых срабатываниях или при первой итерации, когда _prev ещё не инициализирован и его Time = 0), то знаменатель равен нулю
			if (float.IsNaN(delta)) delta = 0f;
			//todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			// Vector3.Lerp  возвращает точку на отрезке между _prev.Position и curr.Position. Delta показывает на сколько продвинулись от одной точки к другой
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}