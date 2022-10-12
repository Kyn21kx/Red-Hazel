using Auxiliars;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedBloodHood {
	public class EnemyBehaviour : Entity {

		private const float VFX_FLASH_TIME_MS = 100f;

		public int health = 100;

		private Material matReference;

		private SpartanTimer damageVfxTimer;

		protected override void OnCreate() {
			base.OnCreate();
			StaticMeshComponent renderer = this.GetComponent<StaticMeshComponent>();
			matReference = renderer.GetMaterial(0);
			this.damageVfxTimer = new SpartanTimer(TimeMode.Framed);
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			if (damageVfxTimer.Started && damageVfxTimer.CurrentTimeMS > VFX_FLASH_TIME_MS) {
				this.ChangeColorBackToNormal();
			}
		}

		protected override void OnDestroy() {
			base.OnDestroy();
			this.ChangeColorBackToNormal();
		}

		public void Damage(int amount) {
			health -= amount;
			//Change the color of the material for a lil bit
			this.ChangeColorDamaged();
			//Check if this damage killed the enemy
			if (health <= 0) 
				Destroy();
		}


		private void ChangeColorDamaged() {
			this.matReference.AlbedoColor = Color.Red;
			this.damageVfxTimer.Reset();
		}

		private void ChangeColorBackToNormal() {
			this.matReference.AlbedoColor = Color.White;
			this.damageVfxTimer.Stop();
		}

	}
}
