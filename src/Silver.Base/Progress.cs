namespace Silver
{
    public class ProgressTracker : Runtime
    {
        public ProgressTracker(int total, Action<double> updateAction) : base()
        {
            this.total = total;
            this.updateAction = updateAction;
        }

        
        public void Update()
        {
            this.updateAction((double)completed);
        }
        public void Update(double u) => updateAction(u);


        double total;
        double completed = 0;
        Action<double> updateAction;
    }
}
