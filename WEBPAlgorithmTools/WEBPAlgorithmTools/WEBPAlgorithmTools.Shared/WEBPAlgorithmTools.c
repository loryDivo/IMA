#include "WEBPAlgorithmTools.h"
#include "../../examples/cwebp.h"

	int WEBPEncode_internal(const char * imageSource, const char * imageDestination) {
		char ** string = NULL;
		string[0] = "cwebp";
		string[1] = imageSource;
		string[2] = "-o";
		string[3] = imageDestination;
		return cwebp(4, string);
	}
