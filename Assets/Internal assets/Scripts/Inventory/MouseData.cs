using UnityEngine;
namespace Inventory
{
    public static class MouseData
    {
        /// <summary>
        /// Интерфейс Мышь закончилась
        /// </summary>
        public static UserInterface InterfaceMouseIsOver;
        /// <summary>
        /// Перетаскиваемый временный элемент
        /// </summary>
        public static GameObject TempItemBeingDragged;
        /// <summary>
        /// Слот завис над
        /// </summary>
        public static GameObject SlotHoveredOver;
    }
}