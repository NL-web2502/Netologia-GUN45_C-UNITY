using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;
		
		//todo comment: Что произойдёт, если _delay > _duration? 
		// За время _duration не успеет накопиться ни одной точки (или накопится одна в самом начале).
		[Range(0.2f, 1.0f)]
		private float _delay = 0.5f;
		[SerializeField, Min(0.2f)]
		private float _duration = 5f;

		private void Start()
		{
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			//  Вызывать GetComponent относительно дорогая операция (поиск компонента среди всех компонентов GameObject) и вызывать каждый кард в Update - затраты производительности. 
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();

			 if (_duration <= _delay)
    		{
        		_duration = _delay * 5f;
    		}
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}
			
			//todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
			//  _delay - это конфигурируемое значение интервала. Если уменьшить, то после первого срабатывания он станет другим, и интервал между точками начинает меняться. 
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени? 
					// чтобы знать в какой момент времени была сделана каждая точка
					Time = Time.time,
				});
			}
		}
	}
}