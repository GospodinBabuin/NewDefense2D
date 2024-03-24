using Buildings;
using UnityEngine;

namespace UI
{
    public class AltarMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] potionsLvl1;
        [SerializeField] private GameObject[] potionsLvl2;
        [SerializeField] private GameObject[] potionsLvl3;

        public Altar CurrentAltar { get; private set; }
        
        public void ShowMenu(int buildingLvl, Altar currentAltar)
        {
            CurrentAltar = currentAltar;
            
            switch (buildingLvl)
            {
                case 1:
                    ActivateSlots(potionsLvl1);

                    DeactivateSlots(potionsLvl2);
                    DeactivateSlots(potionsLvl3);
                    break;
                case 2:
                    ActivateSlots(potionsLvl1);
                    ActivateSlots(potionsLvl2);

                    DeactivateSlots(potionsLvl3);
                    break;
                case 3:
                    ActivateSlots(potionsLvl1);
                    ActivateSlots(potionsLvl2);
                    ActivateSlots(potionsLvl3);
                    break;
            }
        }

        private static void ActivateSlots(GameObject[] slots)
        {
            foreach (GameObject slot in slots)
                slot.SetActive(true);
        }
    
        private static void DeactivateSlots(GameObject[] slots)
        {
            foreach (GameObject slot in slots)
                slot.SetActive(false);
        }
    }
}
