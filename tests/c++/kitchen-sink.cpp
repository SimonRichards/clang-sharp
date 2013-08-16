#include "quoted-header.h"
#include <angle-bracketed-header>

#define MACRO(a,b,c) (a + b * c)
#define CONSTANT 42

// Line comment

/* Paragraph comment

*/

namespace {
	const int foo = 123;
}

class ClassOne {
	int a;
	int b;

private:

public:
	ClassOne(int a, int b) :
		a(a),
		b(b) { 
			a = b;
	}
}

// namespace parentnamespace {
namespace test {


	/**
	  * Class is documented.
	  */
	class ClassTwo {

		// Method explanation
		char *GetStuff const(int arg1, const ClassOne &arg2); // trailing comment

	}
}

//}
 // Adjacent comment

struct diff_words_buffer {
	mmfile_t text;
	long alloc;
	struct diff_words_orig {
		const char *begin, *end;
	} *orig;
	int orig_nr, orig_alloc;
};