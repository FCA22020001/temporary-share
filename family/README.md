# parent.cs

```cs
namespace famiry{
    public class Parent : Monobehavior{
        [HideInInspector] public Child child;
        [HideInInspector] public Support support;

        private void Awake(){
            child = GetComponent<Child>();
            support = GetComponent<Support>();
        }

        private void Start(){
            Program();
        }

        private void Program(){
            if (support.human == true){
                child.childCall();
            }
        }
    }
}

```
