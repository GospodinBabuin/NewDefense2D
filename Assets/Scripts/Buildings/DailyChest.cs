using System;
using Interfaces;
using UnityEngine;

namespace Buildings
{
    public class DailyChest : Chest, IInteractable
    {
        public int GoldInChest { get; set; }
        
        private int _animIDSpawn;

        protected override void Awake()
        {
            base.Awake();
            _animIDSpawn = Animator.StringToHash("Spawn");
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void Interact(GameObject interactingObject)
        {
            CollectGold(GoldInChest);
        }

        private void OnEnable()
        {
            animator.SetTrigger(_animIDSpawn);
        }

        public void OnGoldSpawnedAnimEvent()
        {
            animator.SetTrigger(animIDDestroy);
        }

        public void DisableChest()
        {
            gameObject.SetActive(false);
        }
    }
}
