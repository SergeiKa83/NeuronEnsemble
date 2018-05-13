using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeuronEnsemble
{
    class EnsembleGraphNode
    {
        public List<Neuron> ensemble;
        public int length;
        public List<EnsembleGraphNode> consistFrom;
        public List<EnsembleGraphNode> belongTo;
        public EnsembleGraphNode(List<Neuron> e, List<EnsembleGraphNode> cF, List<EnsembleGraphNode> bT)
        {
            ensemble = e;
            consistFrom = cF;
            belongTo = bT;
        }

        // выдаёт список всех нейронов которые являются входом для ансамбля, но в него не входят
        public List<Neuron> getInputNeurons()
        {
            List<Neuron> ln = new List<Neuron>();
            foreach (Neuron ne in ensemble)
            {
                int found = 0;
                foreach (Weight we in ne.getInConnects())
                {
                    if (!ensemble.Contains(we.getNeuron())) 
                    { 
                        //found = 1; break; 
                        if (!ln.Contains(we.getNeuron())) ln.Add(we.getNeuron());
                    }
                }
                //if (found == 1) ln.Add(ne);
            }
            return ln;
        }
        public string[,] getAutomataTable(CheckedListBox.CheckedItemCollection numInputs)
        {
            int mCols = (int)Math.Pow(2, ensemble.Count) + 1; // number of columns - conditions
            int mRows = (int)Math.Pow(2, numInputs.Count) + 1; // number of rows - inputs
            string[,] table = new string[mRows, mCols];
            
            // set the rows
            long genNum = 0;            
            for (int i = 0; i < mRows - 1; i++)
            {
                string bin = Convert.ToString(genNum, 2);
                while (bin.Length < numInputs.Count)
                {
                    bin = "0" + bin;
                }
                //string mirrorbin = null;
                //for (int k = bin.Length - 1; k >= 0; k--) mirrorbin += bin[k].ToString();
                table[i+1,0] = bin;
                
                genNum = genNum + 1;                
            }
            
            // set the columns
            genNum = 0;
            for (int i = 0; i < mCols - 1; i++)
            {
                string bin = Convert.ToString(genNum, 2);
                while (bin.Length < ensemble.Count)
                {
                    bin = "0" + bin;
                }
                
                table[0,i + 1] = bin;
                genNum = genNum + 1;
            }

            // set the content
            for (int i = 1; i <= mRows - 1; i++)
                for (int j = 1; j <= mCols - 1; j++)
                {
                    // set the values of inputs
                    int startIndex = 0;
                    foreach (Neuron neur in this.getInputNeurons()) 
                    {
                        for (int index = 0; index < numInputs.Count; index++) {
                            if (neur.getId() == Convert.ToInt32(numInputs[index].ToString()))
                            {
                                neur.setOutValue(Convert.ToInt32(table[i, 0].Substring(startIndex, 1)));
                                startIndex++;
                                break;
                            }
                            else {
                                neur.setOutValue(0);
                            }
                            //neur.setOutValue(Convert.ToInt32(table[i, 0].Substring(startIndex, 1)));                             
                        }
                        //neur.setOutValue(Convert.ToInt32(table[i, 0].Substring(startIndex, 1))); 
                        //startIndex++; 
                    }
                    
                    // set the values of ensemble elements
                    startIndex = 0;
                    foreach (Neuron neur in this.ensemble) { neur.setOutValue(Convert.ToInt32(table[0, j].Substring(startIndex, 1))); startIndex++; }

                    // run for ensemble
                    foreach (Neuron neuron in ensemble) neuron.Prepare();
                    foreach (Neuron neuron in ensemble) neuron.Run();

                    // get the column (condition)
                    string tcol = "";
                    foreach (Neuron neuron in ensemble) tcol = tcol + neuron.getOutValue().ToString();
                    table[i, j] = tcol;
                }
            return table;
        }
        public int getAutomataHeigh(CheckedListBox.CheckedItemCollection numInputs)
        {
            return (int)Math.Pow(2, numInputs.Count) + 1;
        }
        public int getAutomataWidth()
        {
            return (int)Math.Pow(2, ensemble.Count) + 1;
        }
    }

    class NeuronExt
    {
        public Neuron neuron;
        public List<NeuronExt> neighbors = new List<NeuronExt>();
        public NeuronExt(Neuron _neuron)
        {
            neuron = _neuron;
        }
        public void addNeighbor(NeuronExt neuron)
        {
            /*if (neighbors!=null)
            {*/
                if (neighbors.Exists(x => x.neuron.getId() == neuron.neuron.getId()) == false) neighbors.Add(neuron);
            /*}
            else neighbors.Add(neuron);*/
        }
        public List<NeuronExt> getNeighbors()
        {
            return neighbors;
        }
    }
    class NeuralNet
    {
        public bool isChenged;
        private Neuron[] net;
        private int numRepeatsStartValue;
        public NeuralNet(double[,] w, double[] treshhold, int n, int[] ids, int[] startValues, int numRepeats)
        {
            isChenged = false;
            // n число строк
            // m число столбцов
            net = new Neuron[n];
            numRepeatsStartValue = numRepeats;
            for (int i = 0; i < n; i++) { Neuron neur = new Neuron(ids[i], treshhold[i], startValues[i]); net[i] = neur; }
            for (int j = 0; j < n; j++)
            {
                List<Weight> wList = new List<Weight>();
                for (int i = 0; i < n; i++)
                {
                    if (w[i,j]!=0)
                    { 
                        Weight ww = new Weight(net[i], w[i,j]);
                        wList.Add(ww);
                    }
                }
                net[j].addInConnects(wList);
            }
        }
        public NeuralNet() { }
        public void Step()
        {
            foreach (Neuron neuron in net) neuron.Prepare();
            foreach (Neuron neuron in net) neuron.Run();
        }

        public int[] getValues()
        {
            int[] res = new int[net.Length];
            for (int i = 0; i < net.Length; i++) res[i] = net[i].getValue();           
            return res;
        }

        public double[,] TimeSteps(int time)
        {
            double[,] vals = new double[net.Length, time];            
            for (int t = 0; t < time; t++)
            {
                this.Step();
                int[] res = this.getValues();
                for (int i = 0; i < net.Length; i++) vals[i, t] = res[i];
            }
            return vals;
        }
        public List<List<Neuron>> getEnsembles()
        {
            isChenged = false;
            // сохраняем начальные значения в сети
            int index=0;
            int[] outVal = new int[this.net.Length];
            foreach(Neuron ne in net)
            {
                outVal[index] = ne.getOutValue();
                index++;
            }

            int m = (int)Math.Pow(2, net.Length);           
            List<List<Neuron>> ensembles = new List<List<Neuron>>();
            List<Neuron> listOfNeurons = new List<Neuron>();
            long genNum=1;
            for (int i=0; i<m-1; i++)
            {
                string bin = Convert.ToString(genNum, 2);
                string mirrorbin = null;
                for (int k = bin.Length - 1; k >= 0; k--) mirrorbin += bin[k].ToString();
                for (int j = 0; j < mirrorbin.Length; j++)
                {
                    if (mirrorbin[j] == '1')
                    {
                        listOfNeurons.Add(net[j]);
                    }
                }
                genNum = genNum + 1;
                if (isEnsemble(listOfNeurons))
                {
                    ensembles.Add(listOfNeurons);
                }
                //listOfNeurons.Clear();
                listOfNeurons = new List<Neuron>();
            }

            // возвращаем стартовые значения
            index = 0;
            foreach (Neuron ne in net)
            {
                ne.setOutValue(outVal[index]);
                index++;
            }

            return ensembles;            
        }        

        public void _Step(Neuron[] listOfNeurons)
        {
            foreach (Neuron neuron in net) if (!listOfNeurons.Contains(neuron)) neuron.setOutValue(0);
            /* вставка для замыкания
            foreach (Neuron neuron in listOfNeurons) neuron.Prepare();
            foreach (Neuron neuron in listOfNeurons) neuron.Run();
             */
            foreach (Neuron neuron in net) neuron.Prepare();
            foreach (Neuron neuron in net) neuron.Run();
        }

        public bool isEnsemble(List<Neuron> list)
        {
            Neuron[] listOfNeurons = new Neuron[list.Count];
            int mi = 0;
            foreach (Neuron nn in list)
            {
                listOfNeurons[mi] = nn;
                mi = mi + 1;
            }
            if (isConnective(listOfNeurons))
            {
                foreach (Neuron n in listOfNeurons)
                {
                    n.setOutValue(1);
                }
                this._Step(listOfNeurons);
                
                // условие замыкания***
                foreach (Neuron neuron in net)
                    if ((!listOfNeurons.Contains(neuron)) && neuron.getValue() == 1)
                        return false;
                //***
                
                foreach (Neuron n in listOfNeurons)
                {
                    if (n.getOutValue() == 0)
                        return false;
                }
                this._Step(listOfNeurons);
                
                // условие замыкания***
                foreach (Neuron neuron in net)
                    if ((!listOfNeurons.Contains(neuron)) && neuron.getValue() == 1)
                        return false;
                //***
                
                foreach (Neuron n in listOfNeurons)
                {
                    if (n.getOutValue() == 0)
                        return false;
                }
                return true;
            }
            else return false;
        }
        
        public bool isConnective(Neuron[] listOfNeurons)
        {
            if (listOfNeurons.Length > 0)
            {
                // формируем неориентированный граф связности
                NeuronExt[] listOfExtNeurons = new NeuronExt[listOfNeurons.Length];
                // формируем список Ext-нейронов
                for (int i = 0; i < listOfNeurons.Length; i++)
                {
                    listOfExtNeurons[i] = new NeuronExt(listOfNeurons[i]);
                }
                // добавляем соседей по входам и замыкаем на выходы
                for (int i = 0; i < listOfNeurons.Length; i++)
                {
                    // добавили самого себя ?
                    //listOfExtNeurons[i].addNeighbor(listOfExtNeurons[i]);
                    // добавляем соседей
                    foreach (Neuron nn in listOfExtNeurons[i].neuron.getNeighbors())
                    {
                        //listOfExtNeurons[i].addNeighbor(nn);
                        // добавляем соседям текущий нейрон как соседа
                        for (int k = 0; k < listOfNeurons.Length; k++)
                        {
                            if (listOfExtNeurons[k].neuron.getId() == nn.getId())
                            {
                                listOfExtNeurons[k].addNeighbor(listOfExtNeurons[i]);
                                listOfExtNeurons[i].addNeighbor(listOfExtNeurons[k]);
                            }
                        }
                    }
                }
                // проверяем связность графа
                List<NeuronExt> isMarked = new List<NeuronExt>();
                List<NeuronExt> queue = new List<NeuronExt>();
                queue.Add(listOfExtNeurons[0]);
                bool itIsConnective = false;
                while (queue.Count > 0)
                {
                    // берем первый нейрон в очереди
                    NeuronExt tekN = queue.First<NeuronExt>();
                    queue.Remove(tekN);
                    // помечаем его
                    isMarked.Add(tekN);
                    if (isMarked.Count == listOfNeurons.Length) // всё, связный
                    {
                        itIsConnective = true;
                        break;
                    }
                    // добавляем в очередь всех его непомеченных соседей
                    foreach (NeuronExt neur in tekN.neighbors)
                    {
                        if ((queue.Exists(x => x.neuron.getId() == neur.neuron.getId()) == false) &&
                            (isMarked.Exists(x => x.neuron.getId() == neur.neuron.getId()) == false)) queue.Add(neur);
                    }
                }
                return itIsConnective;
            }
            else
                return false;
        }
        public Neuron[] getNet()
        {
            return net;
        }

        public bool belongTo(List<Neuron> a, List<Neuron> b) // a.Count < b.Count
        {
            bool res = true;
            foreach (Neuron na in a)
            {
                if (b.Exists(x => x.getId() == na.getId()) == false)
                {
                    res = false;
                    break;
                }
            }
            return res;
        }

        /*private void checkNodes(ref EnsembleGraphNode en1, ref EnsembleGraphNode en2)
        {
            foreach()
        }*/

        private string getNameOfEns(List<Neuron> en)
        {
            string str="";
            foreach (Neuron ln2 in en)
            {
                str = str + "-" + ln2.getId().ToString();
            }
            str = str.Substring(1, str.Length - 1);
            return str;
        }

        private TreeNode getTreeNode(EnsembleGraphNode egN)
        {
            TreeNode tekTN = new TreeNode(getNameOfEns(egN.ensemble));
            tekTN.Tag = egN;
            foreach(EnsembleGraphNode egN2 in egN.consistFrom) tekTN.Nodes.Add(getTreeNode(egN2));           
            return tekTN;
        }

        public void getTreeView(TreeNodeCollection tv)
        {
            //TreeNodeCollection tv = new TreeNodeCollection();
            List<List<Neuron>> listEs = getEnsembles();
            List<EnsembleGraphNode> eG = new List<EnsembleGraphNode>();
            foreach (List<Neuron> ln in listEs)
            {
                eG.Add(new EnsembleGraphNode(ln,new List<EnsembleGraphNode>(), new List<EnsembleGraphNode>()));
            }
            // формируем граф включений
            foreach (List<Neuron> ln in listEs)
            {
                foreach (List<Neuron> ln2 in listEs)
                {
                    if (ln2.Count < ln.Count)
                    {
                        if (belongTo(ln2, ln))
                        {
                            EnsembleGraphNode a = eG.Find(x => x.ensemble == ln2);
                            EnsembleGraphNode b = eG.Find(x => x.ensemble == ln);
                            if (a.belongTo.Exists(x => x.ensemble == b.ensemble) == false) a.belongTo.Add(b);
                            if (b.consistFrom.Exists(x => x.ensemble == a.ensemble) == false) b.consistFrom.Add(a);
                        }
                    }
                }
            }
            // редактируем граф включений (удаляем опосредованные включения)
            foreach (EnsembleGraphNode en1 in eG)
            {
                en1.length = en1.ensemble.Count;
                foreach (EnsembleGraphNode en2 in eG)
                {                    
                    IEnumerable<EnsembleGraphNode> intersection = en1.belongTo.Intersect(en2.belongTo);
                    if(en1.belongTo.Exists(x => x == en2) == true)
                        foreach (EnsembleGraphNode gNode in intersection.ToArray<EnsembleGraphNode>())
                        {
                            en1.belongTo.Remove(gNode);
                            gNode.consistFrom.Remove(en1);
                        }
                    else if (en2.belongTo.Exists(x => x == en1) == true)
                        foreach (EnsembleGraphNode gNode in intersection)
                        {
                            en2.belongTo.Remove(gNode);
                            gNode.consistFrom.Remove(en2);
                        }
                }
            }
            // getNameOfEns            
            foreach (EnsembleGraphNode ln in eG)
            {
                if(ln.belongTo.Count==0)
                   tv.Add(getTreeNode(ln));
            }
            //return tv;
        }
    }
}
