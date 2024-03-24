using Buildings;
using UnityEngine;

namespace UI
{
    public class UnitMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] unitsLvl1;
        [SerializeField] private GameObject[] unitsLvl2;
        [SerializeField] private GameObject[] unitsLvl3;


        public Barracks CurrentBarack { get; private set; }
    
        public void ShowMenu(int buildingLvl, Barracks currentBarack)
        {
            CurrentBarack = currentBarack;
            
            switch (buildingLvl)
            {
                case 1:
                    ActivateSlots(unitsLvl1);
                
                    DeactivateSlots(unitsLvl2);
                    DeactivateSlots(unitsLvl3);
                    break;
                case 2:
                    ActivateSlots(unitsLvl1);
                    ActivateSlots(unitsLvl2);
                
                    DeactivateSlots(unitsLvl3);
                    break;
                case 3:
                    ActivateSlots(unitsLvl1);
                    ActivateSlots(unitsLvl2);
                    ActivateSlots(unitsLvl3);
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
