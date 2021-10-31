using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueSkills.Enums
{
    public enum Step
    {
        /// <summary>
        /// Экзамен еще на начался
        /// </summary>
        ExamNotRun = 1,
        /// <summary>
        /// Экзамен начался, отображение документов
        /// </summary>
        ExamHasStartedDocumentDisplayed = 2,
        /// <summary>
        /// Экзамен начался, модуль еще не начался
        /// </summary>
        ExamHasStartedModuleNotStarted = 3,
        /// <summary>
        /// Экзамен начался, отображение задания
        /// </summary>
        ExamStartTaskDisplay = 4,
        /// <summary>
        /// Экзамен начался, модуль идет
        /// </summary>
        ExamStartModuleUnderway = 5,
        /// <summary>
        /// Экзамен закончен
        /// </summary>
        ExamOver = 6,
    }
}
