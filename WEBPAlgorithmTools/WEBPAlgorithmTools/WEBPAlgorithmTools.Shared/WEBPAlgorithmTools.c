#include "WEBPAlgorithmTools.h"
#include "../../examples/cwebp.h"

int WEBPEncode_internal(const char * imageSource, const char * imageDestination) {
	const char * string[4];
	string[0] = "cwebp";
	string[1] = imageSource;
	string[2] = "-o";
	string[3] = imageDestination;
	return cwebp(4, string);
}


