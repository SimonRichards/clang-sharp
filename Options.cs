using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClangSharp {
    // ReSharper disable InconsistentNaming
    public enum Options {
        Werror,
        w,
        Weverything,
        pedantic,
        pedantic_errors,
        Wsystem_errors,
        fno_show_column,
        fno_show_source_location,
        fno_caret_diagnostics,
        fno_color_diagnostics,
        fdiagnostics_show_name,
        fno_diagnostics_show_option,
        fno_diagnostics_fixit_info,
        fdiagnostics_print_source_range_info,
        fdiagnostics_parseable_fixits,
        fno_elide_type,
        fdiagnostics_show_template_tree,
        Wextra_tokens,
        Wambiguous_member_template,
        Wbind_to_temporary_copy,
        fcatch_undefined_behavior,
        faddress_sanitizer,
        fthread_sanitizer,
        fno_assume_sane_operator_new,
        g0,
        gline_tables_only,
        g
    }
    // ReSharper restore InconsistentNaming
}
