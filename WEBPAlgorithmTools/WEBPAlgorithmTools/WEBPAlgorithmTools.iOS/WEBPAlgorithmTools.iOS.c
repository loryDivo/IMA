#include "WEBPAlgorithmTools.h"

int WEBPEncode(const char * imageSource, const char * imageDestination, const char *quality) {
	return WEBPEncode_internal(imageSource, imageDestination, quality);
}