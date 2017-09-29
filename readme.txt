A comprehensive set of developer documentation for the codebase can be found in DoxygenGuide

athough it should be pretty straightforward to get the visual studio solution to run.

The most important things are that:

1. LISA is installed on your local machine and the windows path variable has been configured 
so the project can call it directly from the command line. If trying to run more than a few iterations LISA will 
throw an error as the program tries to make more than 1300 elements at which point no additional meshes will be generated.

2. Make sure the paths to the liml files and experiment folders are also configured for your local machine, 
currently these are hard coded within the Control and Program classes.

3. Make sure you have the latest version of .Net installed
