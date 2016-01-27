import java.net.URL;
import java.io.File;
import java.net.URLClassLoader;
import java.lang.reflect.Method;
import java.io.PrintStream;

public class ForgeInstallWrapper {
  private static File installerJar;
  private static URLClassLoader loader;
  @SuppressWarnings("rawtypes")
  private static Class installerClass;
  private static Method runnerMethod;

  public static void main(String[] args) {
	  if (args.length == 2)
	  {
		ForgeInstall(new File(args[0]), new File(args[1]));
	  }
	  else
	  {
		System.out.println("Incorrect parameters");
	  }
  }
  
  @SuppressWarnings("unchecked")
  public static void ForgeInstall(File _installerJar, File minecraftDir) {
    installerJar = _installerJar;

    System.out.println("Installing Forge from JAR");

    try {
      URL[] jarPath = { installerJar.toURI().toURL() };
      loader = new URLClassLoader(jarPath, ForgeInstallWrapper.class.getClassLoader());
      installerClass = Class.forName("net.minecraftforge.installer.ClientInstall", true, loader);
      runnerMethod = installerClass.getMethod("run", File.class);
    } catch (Throwable e) {
      e.printStackTrace();
    }

    try {
      Object instance = installerClass.newInstance();
      Object result = runnerMethod.invoke(instance, minecraftDir);
    } catch (Throwable e) {
      System.out.println("Unable to install Forge");
      System.exit(-1);
    }

    System.out.println("Forge update complete");
  }
}
