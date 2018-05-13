using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronEnsemble
{
    class Weight
    {
        private double weight;
        private Neuron neuron;
        private int value; // входное значение на дендрит или выходное значение на аксон в зависимости от списка
        public Weight(Neuron n, double w) { neuron = n; weight = w; }
        public void setWeight(double w) { weight = w; }
        public void setValue(int val)        { value = val; }
        public double getWeight() { return weight; }
        public int getValue() { return value; }
        public Neuron getNeuron() { return neuron; }
    }
    class Neuron
    {
        private int id;
        private double treshhold;
        private List<Weight> inConnects;
        private int outValue;
        //private List<Weight> outConnects;

        public int getValue() { return this.outValue; }

        public Neuron(int _id, double _treshhold, int _outValue)
        {
            id = _id;
            treshhold = _treshhold;
            outValue = _outValue;
        }

        public int getId() { return id; }

        public double getTreshhold() { return treshhold; }  

        public void addInConnects(List<Weight> list)
        {
            inConnects = list;
        }       
               
        // изменение значения входа нейрона
        /*public void setInValue(int id, int value) // передает свой id и value
        {
            foreach (Weight conn in inConnects)
            {
                if (conn.getNeuron().getId() == id) // тот самый нейрон
                    conn.setValue(value); 
            }
        }*/

        public int getOutValue()
        { return outValue;  }

        // подготовка нейрона к запуску
        public void Prepare()
        {
            // перекидываем значения из выходной части нейрона во входную часть
            foreach (Weight con in inConnects)   con.setValue(con.getNeuron().getOutValue());
        }

        // расчет значения нейрона на новом такте
        public void Run()
        {
            double result = 0;
            foreach (Weight conn in inConnects)
            {
                result = result + conn.getWeight() * conn.getValue();
            }
            if (result >= this.treshhold)
                this.outValue = 1;
            else
                this.outValue = 0;
        }
    
        // вернуть своих соседей по входу
        public List<Neuron> getNeighbors()
        {
            List<Neuron> neurons = new List<Neuron>();
            foreach(Weight w in inConnects)
            {
                neurons.Add(w.getNeuron());
            }
            return neurons;
        }

        // установить выходное значение (нужно для поиска ансамблей)
        public void setOutValue(int val)
        {
            this.outValue = val; 
        }

        public List<Weight> getInConnects()
        {
            return inConnects;
        }
    }
}
