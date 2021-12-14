using System.Collections.Generic;
using System.Reflection;

namespace SAM.Analytical.OpenStudio
{
    public static partial class Query
    {
        /// <summary>
        /// Source: https://github.com/NREL/OpenStudio-resources/blob/538b3c7df3f6ed4513f354cff7d96f7c556c267e/model/simulationtests/lib/baseline_model.rb#L612:L655
        /// </summary>
        /// <param name="path_DDY"></param>
        /// <returns></returns>
        public static List<DesignDay> DesignDays(string path_DDY)
        {

            global::OpenStudio.Path openStudioPath = global::OpenStudio.OpenStudioUtilitiesCore.toPath(path_DDY);
            if (openStudioPath == null)
                return null;

            global::OpenStudio.OptionalIdfFile optionalIdfFile = global::OpenStudio.IdfFile.load(openStudioPath, new global::OpenStudio.IddFileType("EnergyPlus"));
            global::OpenStudio.Workspace workspace = new global::OpenStudio.Workspace(optionalIdfFile.get());

            global::OpenStudio.EnergyPlusReverseTranslator energyPlusReverseTranslator = new global::OpenStudio.EnergyPlusReverseTranslator();
            global::OpenStudio.Model model = energyPlusReverseTranslator.translateWorkspace(workspace);
            global::OpenStudio.OptionalDesignDay optionalDesignDay = model.getDesignDayByName("AAA");

            throw new System.NotImplementedException();

        }
    }
}