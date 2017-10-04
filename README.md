# Hybrid Methods for Finite Element Meshing

This project was my third year undergraduate dissertation which I worked on during time at 
the University of Nottingham, this read me provides a bried overview of the dissertation writeup which can be viewed here:
[./psyjb4_Jack_Bradbrook_dissertation.pdf](psyjb4_Jack_Bradbrook_dissertation.pdf)

## Overview

The goal of this project was to design a system by which could apply heuristic rules in conjunction with another already proven method
in order to speed up an often slow numerical method prominent across various engineering domains called "Finite Element Analysis"

The Finite Element(FE) Method takes some space represented as a mesh structure and attempts to calcuate some property across the mesh using
equations which can be used to determine the distribution of that property across the mesh space 
after running the finite element method. 

The speed and accuracy of an FE simulation is largely determined by the number of elements - cells within the mesh,

* more elements -> more calculations which need to be performed.

* more cells in an area -> greater accuracy of results for that area

It is therefore advantagious to have more elements within the mesh in areas where higher accuracy is required and fewer in areas where it is not.

Normally to determine where the mesh should be refined to a higher fidelity several iterations are run of the model with gradually finer meshes
so as to establish which part of the mesh is of greatest interest, once this is done further refinement can be focused exclusively in those areas.

![Alt text](./WriteUp/Graphics/StressedCorner.png "Optional Title")

For this project I looked at the possibility of speeding up this process further by building a system which could store and apply heuristics that describe
where it would be most benificial for the mesh to be refined. These could could then be applied to the mesh refining sections of importance
without needing to rely on the traditional approach which would run several solve iterations first to determine these areas.

Finally using both methods it was then possible apply each of different amounts to a series of models to attempt to determine how effective each approach was
and to what degree.

## Tasks / Methodology

The project can be broken down into several key components:

1. Create Interface with finite element solver which given a mesh and some additional parameters for describing the problem to be solved
for this project I focused on the application of FE to structural analysis problems

2. Build internal data structure to represent mesh and write a traditional stress based refinement method

3. build a heuristic refinement method which could use both engineering knowledge about a particular problem and inherent 

4. Analyse models, see what the improvement in accuracy was for each method and at what cost in terms of time overhead and elements created  


## Design of project
To allow for flexible design in order to incorporate required changes from experimentation the software was broken down

For solving each mesh I used a finite element application known as [LISA](http://www.lisa-fet.com/) 
Interfacing with LISA was achieved through reading and writing LISA input and output files (.liml) 
then calling LISA with this as a parameter

### Mesh Structure

The internal structure representing the mesh had to be both easy to modify by the different refinement processes and
flexible so as to support almost any mesh structure.

So the solution could generalise for different types of meshes it needed to support different shapes of elemets.
The element shapes selected by FE engineers can vary from model to model, for example to perform analysis of 
stress

To reslove this problem the solution a class higherarchy can be used

![Alt text](./WriteUp/Graphics/ElementHigerarchyDiagram5.png "Optional Title")


![Alt text](./WriteUp/Graphics/LISA-quad4.png "Quad4 element")
![Alt text](./WriteUp/Graphics/LISA-hex8.png "Hex8 element")
![Alt text](./WriteUp/Graphics/LISA-tri3.png "Tri3 element")
![Alt text](./WriteUp/Graphics/LISA-tet4.png "Tet4 Element")
![Alt text](./WriteUp/Graphics/LISA-line2.png "Line2 Element")
![Alt text](./WriteUp/Graphics/LISA-line3.png "Line3 Element")

This higerarchy simplified the architecture by allowing the code used to refine each of the elements into sub elements and calculate element metrics able to be encapsulated within each respective element class


## Stress Refinement


### Heuristic Refinement

The heuristic knowledge for solving this problem would encompass the following:

* Knowledge an engineer using the tool knows about its
*

## Analysis


Reliable finite element analysis of components



This project required multiple iterative stages of research and development before completion, 
for the detailed dissertation write up that describes the project in more detail, see 



![Alt text](/relative/path/to/img.jpg?raw=true "Optional Title")


## Getting the project running

It shouldn't be too difficult to set up and make modifications to the software, the whole application can 
be loaded easily within Visual Studio using "DissertationFEPrototype.sln" from where documentation describing 
the workings of the different modules is clearly described using C# doc comments. 

Alternatively the doc comments have also been used to generate a full set of developer documentation using the tool Doxygen

The most important things are that:

1. LISA is installed on your local machine and the windows path variable has been configured 
so the project can call it directly from the command line. 
Note: LISA is proprietary software with the trial version free and the full version which allows creation of more than 1300 elements requiring purchase.
This means when you run the system LISA will throw an error to it after several iterations at which point no additional meshes will be generated.

2. Make sure the paths to the liml files and experiment folders are also configured for your local machine, 
currently these are hard coded within the Control and Program classes.

3. Make sure you have at least .Net 4.0 installed
