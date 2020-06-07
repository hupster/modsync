javac -cp forge-1.12.1-14.22.1.2478-installer.jar ForgeInstallWrapper.java
jar -cf ForgeInstallWrapper.jar *.class
del *.class

javac -cp forge-1.12.2-14.23.5.2854-installer.jar ForgeInstallWrapper2.java
jar -cf ForgeInstallWrapper2.jar *.class
del *.class
