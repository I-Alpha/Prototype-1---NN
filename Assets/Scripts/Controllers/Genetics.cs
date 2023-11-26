 
using UnityEngine;


public class Genetics
{
    public static float mutationRate = 0.01f; // Chance of mutation

    public static string[] Breed(string[] parent1Genes, string[] parent2Genes)
    {
        // Combine genes and apply mutations
        string[] childGenes = new string[parent1Genes.Length];
        for (int i = 0; i < childGenes.Length; i++)
        {
            childGenes[i] =  Random.value < 0.5f ? parent1Genes[i] : parent2Genes[i];
            // Apply mutation based on some probability
            if ( Random.value < mutationRate)
            {
                childGenes[i] = MutateGene(childGenes[i]);
            }
        }
        return childGenes;
    }

    private static string MutateGene(string gene)
    {
        // Mutation logic
        return gene; // Return the mutated gene
    }


}
[System.Serializable]
public class GeneticTraits
{
    public Color color; // Color of the agent
    public float size; // Size of the agent
    public float speed; // Speed of the agent

    // You can add more traits as needed
}

[System.Serializable]
public class GeneticData
{
    public string dna; // A string representing the genetic code, e.g., "AaBbCc"

    // Parameterless constructor
    public GeneticData()
    {
        // You can initialize the dna with a default value or leave it empty
        dna = "";
    }

    // Constructor to randomize DNA for initial population or offspring
    public GeneticData(int length)
    {
        dna = "";
        for (int i = 0; i < length; i++)
        {
            // Randomly choose between 'A' or 'a', 'B' or 'b', etc.
            dna += (char)Random.Range('A', 'C'); // Upper case for dominant
            dna += (char)Random.Range('a', 'c'); // Lower case for recessive
        }
    }

    // A method to combine DNA from two parents
    public static GeneticData Combine(GeneticData parent1, GeneticData parent2)
    {
        // Assuming both DNA strings are of the same length and properly paired
        GeneticData childData = new GeneticData();
        for (int i = 0; i < parent1.dna.Length; i += 2)
        {
            // For each gene pair, randomly take one gene from each parent
            childData.dna += parent1.dna[Random.Range(i, i + 2)];
            childData.dna += parent2.dna[Random.Range(i, i + 2)];
        }
        return childData;
    }


    // Method to decode DNA into actual traits
    public GeneticTraits Decode()
    {
        GeneticTraits traits = new GeneticTraits();

        // Decode each gene. Example: 'A' = fast, 'a' = slow
        for (int i = 0; i < dna.Length; i += 2)
        {
            switch (dna.Substring(i, 2))
            {
                case "AA":
                    traits.speed += 2; // Fastest
                    break;
                case "Aa":
                case "aA":
                    traits.speed += 1; // Medium speed
                    break;
                case "aa":
                    // No change, slowest
                    break;
                    // Add cases for other genes
            }
        }

        // Set other traits based on genes
        // ...

        return traits;
    }
}