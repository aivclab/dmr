using UnityEditor;
using UnityEngine;

namespace RandomFromDistributions.Examples.GenerateObjects_Example.Editor {
	[CustomEditor(typeof(GenerateObjects))]
	public class GenerateObjectsEditor : UnityEditor.Editor {

		GameObject generated_objects = null;


		public override void OnInspectorGUI() {
			this.DrawDefaultInspector();

			if (this.generated_objects) {
			
				if(GUILayout.Button("Re-Generate Objects")) {

					Undo.DestroyObjectImmediate(this.generated_objects);
					this.generate_objects();
				}
				if(GUILayout.Button("Generate New Objects")) {
				
					Undo.RecordObject (this, "Generate Objects");

					this.generate_objects();
				}
				if(GUILayout.Button("Destroy Last Generated Objects")) {
				
					Undo.RecordObject (this, "Generate Objects");

					Undo.DestroyObjectImmediate(this.generated_objects);
				}

			}
			else {
				if(GUILayout.Button("Generate Objects")) {
				
					Undo.RecordObject (this, "Generate Objects");

					this.generate_objects();
				}
			}

		}
		private void generate_objects() {
			GameObject objects = ((GenerateObjects)this.target).Generate();
			Undo.RegisterCreatedObjectUndo (objects, "Generate objects");
			this.generated_objects = objects;
			EditorUtility.SetDirty( this );
		}
	}
}


