using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Screeps3D.Menus.Main
{
    [RequireComponent(typeof(Button))]
    public abstract class MainMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public abstract string Description { get; }
        public abstract void Invoke();
        
        [SerializeField] private MainMenu _menu = default;
        
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Invoke);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _menu.Description = Description;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_menu.Description == Description)
                _menu.Description = "";
        }
    }
}