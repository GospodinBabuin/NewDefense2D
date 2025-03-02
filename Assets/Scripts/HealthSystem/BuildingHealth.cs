using Buildings;
using Unity.Netcode;
using UnityEngine;

namespace HealthSystem
{
    public class BuildingHealth : Health
    {
        private int _animIDTakeDamageLvl1;
        private int _animIDTakeDamageLvl2;
        private int _animIDTakeDamageLvl3;
        private int _animIDDestroyLvl1;
        private int _animIDDestroyLvl2;
        private int _animIDDestroyLvl3;
        private Building _building;

        protected override void Awake()
        {
            base.Awake();

            _building = GetComponent<Building>();
        }

        protected override void Damage(int damageAmount)
        {
            switch (_building.BuildingLvl.Value)
            {
                case 1:
                    animator.SetTrigger(_animIDTakeDamageLvl1); 
                    break;
                case 2:
                    animator.SetTrigger(_animIDTakeDamageLvl2); 
                    break;
                case 3:
                    animator.SetTrigger(_animIDTakeDamageLvl3); 
                    break;
            }
        
            base.Damage(damageAmount);
        }

        public void TestDestroy()
        {
            Die();
        }

        [ContextMenu("Destroy")]
        protected override void Die()
        {
            switch (_building.BuildingLvl.Value)
            {
                case 1:
                    animator.SetTrigger(_animIDDestroyLvl1); 
                    break;
                case 2:
                    animator.SetTrigger(_animIDDestroyLvl2); 
                    break;
                case 3:
                    animator.SetTrigger(_animIDDestroyLvl3); 
                    break;
            }

            if (gameObject.CompareTag("Base"))
            {
                GameManager.Instance.Defeat();
                
                return;
            }
            
            Destroy(_building);
            
            Collider2D collider2D = GetComponent<Collider2D>();
            collider2D.enabled = false;
        
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;            
            
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length + 5f);
        }

        protected override void SetAnimIDs()
        {
            _animIDTakeDamageLvl1 = Animator.StringToHash("TakeDamageLvl1");
            _animIDTakeDamageLvl2 = Animator.StringToHash("TakeDamageLvl2");
            _animIDTakeDamageLvl3 = Animator.StringToHash("TakeDamageLvl3");
        
            _animIDDestroyLvl1 = Animator.StringToHash("DestroyLvl1");
            _animIDDestroyLvl2 = Animator.StringToHash("DestroyLvl2");
            _animIDDestroyLvl3 = Animator.StringToHash("DestroyLvl3");
        }
    }
}
