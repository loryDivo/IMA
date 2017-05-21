// Copyright 2011 Google Inc. All Rights Reserved.
//
// Use of this source code is governed by a BSD-style license
// that can be found in the COPYING file in the root of the source
// tree. An additional intellectual property rights grant can be found
// in the file PATENTS. All contributing project authors may
// be found in the AUTHORS file in the root of the source tree.
// -----------------------------------------------------------------------------
//
//  simple command line calling the WebPEncode function.
//  Encodes a raw .YUV into WebP bitstream
//
// Author: Skal (pascal.massimino@gmail.com)

#include <stdio.h>
#include <stdlib.h>
#include <string.h>


#include "../src/webp/config.h"


#include "../examples/example_util.h"
#include "../imageio/image_dec.h"
#include "../imageio/imageio_util.h"
#include "../examples/stopwatch.h"
#include "../src/webp/encode.h"

#ifndef WEBP_DLL
#ifdef __cplusplus
extern "C" {
#endif

	extern void* VP8GetCPUInfo;   // opaque forward declaration.

#ifdef __cplusplus
}    // extern "C"
#endif
#endif  // WEBP_DLL

	 //------------------------------------------------------------------------------

static int ReadYUV(const uint8_t* const data, size_t data_size,
	WebPPicture* const pic);

#ifdef HAVE_WINCODEC_H

static int ReadPicture(const char* const filename, WebPPicture* const pic,
	int keep_alpha, Metadata* const metadata);
#else  // !HAVE_WINCODEC_H

static int ReadPicture(const char* const filename, WebPPicture* const pic,
	int keep_alpha, Metadata* const metadata);
#endif  // !HAVE_WINCODEC_H

static void AllocExtraInfo(WebPPicture* const pic);

static void PrintByteCount(const int bytes[4], int total_size,
	int* const totals);

static void PrintPercents(const int counts[4], int total);

static void PrintValues(const int values[4]);

static void PrintFullLosslessInfo(const WebPAuxStats* const stats,
	const char* const description);

static void PrintExtraInfoLossless(const WebPPicture* const pic,
	int short_output,
	const char* const file_name);

static void PrintExtraInfoLossy(const WebPPicture* const pic, int short_output,
	int full_details,
	const char* const file_name);

static void PrintMapInfo(const WebPPicture* const pic);

static int MyWriter(const uint8_t* data, size_t data_size,
	const WebPPicture* const pic);

static int DumpPicture(const WebPPicture* const picture, const char* PGM_name);

static void PrintMetadataInfo(const Metadata* const metadata,
	int metadata_written);

static int WriteLE(FILE* const out, uint32_t val, int num);

static int WriteLE24(FILE* const out, uint32_t val);

static int WriteLE32(FILE* const out, uint32_t val);

static int WriteMetadataChunk(FILE* const out, const char fourcc[4],
	const MetadataPayload* const payload);

static int UpdateFlagsAndSize(const MetadataPayload* const payload,
	int keep, int flag,
	uint32_t* vp8x_flags, uint64_t* metadata_size);

static int WriteWebPWithMetadata(FILE* const out,
		const WebPPicture* const picture,
		const WebPMemoryWriter* const memory_writer,
		const Metadata* const metadata,
		int keep_metadata,
		int* const metadata_written);

static int ProgressReport(int percent, const WebPPicture* const picture);

static void HelpShort(void);

static void HelpLong(void);

int cwebp(int argc, const char *argv[]);