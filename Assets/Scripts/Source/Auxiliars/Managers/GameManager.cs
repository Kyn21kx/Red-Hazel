using Hazel;
using System.Collections.Generic;

namespace RedBloodHood.Auxiliars {
	public class GameManager : Entity {

		private bool isFirstFrame = true;
		private Dictionary<ulong, Entity> enemiesMap;

		//TODO: Maybe move this to an InputManager script
		public int PluggedControllerID { 
			get {
				if (pluggedControllerID == -1 && IsControllerPlugged)
					throw new System.Exception("The controlller has been disconnected");
				return pluggedControllerID;
			} 
			private set => pluggedControllerID = value;
		}

		private int pluggedControllerID;
		public bool IsControllerPlugged { get; private set; }

		protected override void OnCreate() {
			EntityFetcher.player = FindEntityByTag("Player");
			EntityFetcher.gameManager = this;
			isFirstFrame = true;
			this.IsControllerPlugged = false;
			this.enemiesMap = new Dictionary<ulong, Entity>();
		}

		protected override void OnUpdate(float ts) {
			base.OnUpdate(ts);
			HandleControllerInput();
			if (isFirstFrame) {
				FillMaps();
				isFirstFrame = false;
			}

		}

		private void FillMaps() {
			Entity[] allEntities = Scene.GetEntities();
			for (int i = 0; i < allEntities.Length; i++) {
				EnemyMapCheckAndInsert(allEntities[i]);
			}
		}

		private void HandleControllerInput() {
			int[] controllers = Input.GetConnectedControllerIDs();
			if (controllers.Length < 1) {
				pluggedControllerID = -1;
				return;
			}
			this.pluggedControllerID = controllers[0];
			this.IsControllerPlugged = true;
		}

		private void EnemyMapCheckAndInsert(Entity entity) {
			ScriptComponent scriptComponent = entity?.GetComponent<ScriptComponent>();
			EnemyBehaviour behaviour = scriptComponent?.Instance as EnemyBehaviour;
			if (entity == null || scriptComponent == null || behaviour == null) {
				return;
			}
			this.enemiesMap.Add(entity.ID, entity);
		}

		public Entity FindEnemyOrNull(ulong id) {
			bool found = this.enemiesMap.TryGetValue(id, out var entity);
			if (!found) return null;
			return entity;
		}

	}
}
