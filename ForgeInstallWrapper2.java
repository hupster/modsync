import java.net.URL;
import java.io.File;
import java.net.URLClassLoader;
import java.lang.reflect.Method;
import java.io.PrintStream;
import java.util.function.Predicate;
import net.minecraftforge.installer.actions.ClientInstall;
import net.minecraftforge.installer.actions.ProgressCallback;
import net.minecraftforge.installer.json.Install;
import net.minecraftforge.installer.json.Util;

public class ForgeInstallWrapper2 {

	public static void main(String[] args)
	{
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
	public static void ForgeInstall(File _installerJar, File minecraftDir)
	{
		System.out.println("Installing Forge from JAR v2");
		
		try {
		
			Install profile = Util.loadInstallProfile();
			ProgressCallback monitor = ProgressCallback.withOutputs(System.out);
			ClientInstall install = new ClientInstall(profile, monitor);
			
			Predicate<String> optPred = new Predicate<String>() {
				@Override
				public boolean test(String s) {
					return true;
				}
			};
			
			Object result = install.run(minecraftDir, optPred);
		
		} catch (Throwable e1) {

			e1.printStackTrace();
			System.exit(-1);
		}
		
		System.out.println("Forge update complete");
	}
}
