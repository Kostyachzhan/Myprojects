package com.company;

import org.apache.commons.compress.archivers.ArchiveEntry;
import org.apache.commons.compress.archivers.tar.TarArchiveEntry;
import org.apache.commons.compress.archivers.tar.TarArchiveInputStream;
import org.apache.commons.compress.archivers.tar.TarArchiveOutputStream;
import org.apache.commons.compress.compressors.gzip.GzipCompressorInputStream;
import org.apache.commons.compress.compressors.gzip.GzipCompressorOutputStream;
import org.codehaus.plexus.archiver.Archiver;

import java.io.*;
import java.nio.file.*;
import java.nio.file.attribute.BasicFileAttributes;
import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

public class Zip {
    private String Directory;
    private String ArchivePath;
    private boolean CanOverwrite = false;
    private String Output;

    public Zip(String[] args) {
        if(args.length == 0)
        {
            throw new IllegalArgumentException("No mode was selected");
        }
        switch (args[0])
        {
            case "archive":
                String directory = args[1];
                String archive = args[2];
                java.nio.file.Path dpath = Paths.get(directory);
                if(!Files.exists(dpath)) {
                    throw new IllegalArgumentException("Directory doesn't exist");
                }
                if (!Files.isDirectory((dpath)))
                {
                    throw new IllegalArgumentException("Directory doesn't exist");
                }
                java.nio.file.Path aopath = Paths.get(archive);
                if(!Files.exists(aopath)) {
                    throw new IllegalArgumentException("Archive output directory doesn't exist");
                }
                if (!Files.isDirectory((aopath)))
                {
                    throw new IllegalArgumentException("Archive output directory doesn't exist");
                }
                if (Files.exists(aopath))
                {
                    System.out.println("Do you wish to overwrite this file?\ny/N\n");
                    BufferedReader reader = new BufferedReader(
                            new InputStreamReader(System.in));
                    String input = "";
                    try {
                        input = reader.readLine();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                    switch (input)
                    {
                        case "Y":
                        case "y":
                            CanOverwrite = true;
                            break;
                        default:
                            CanOverwrite = false;
                            break;
                    }
                }
                Directory = directory;
                ArchivePath = archive;
                break;
            case "update":
                archive = args[1];
                aopath = Paths.get(archive);
                if (!Files.exists(aopath))
                {
                    throw new IllegalArgumentException("Archive file doesn't exist");
                }
                ArchivePath = archive;
                break;
            case "extract":
                archive = args[1];
                String output = args[2];
                aopath = Paths.get(archive);
                if (!Files.exists(aopath))
                {
                    throw new IllegalArgumentException("Archive file doesn't exist");
                }
                java.nio.file.Path opath = Paths.get(output);
                if (!Files.exists(opath) || !Files.isDirectory((opath)))
                {
                    System.out.println("Output directory doesn't exist. Do you wish to create output directory?\nY/n\n");
                    BufferedReader reader = new BufferedReader(
                            new InputStreamReader(System.in));
                    String input = "";
                    try {
                        input = reader.readLine();
                    } catch (IOException e) {
                        e.printStackTrace();
                    }
                    switch (input)
                    {
                        case "N":
                        case "n":
                            throw new IllegalArgumentException("Output directory doesn't exist");
                        default:
                            try {
                                Files.createDirectory(opath);
                            } catch (IOException e) {
                                e.printStackTrace();
                                throw new IllegalArgumentException("Output directory can't be created, due to access restrictions. Please check output directory path.");
                            }
                        break;
                    }
                }
                ArchivePath = archive;
                Output = output;
                break;
            case "help":
                System.out.println("archive <path to folder to archive> <path to output .tar.gz file with extension>");
                System.out.println("update <path to output .tar.gz file with extension>");
                System.out.println("extract <path to output .tar.gz file with extension> <path to output folder>");
                System.exit(0);
                break;
            default:
                throw new IllegalArgumentException("Command no found");
        }
    }

    public void Archive() throws Exception {
        ArrayList<Path> prepared = GetPreparedFiles(Paths.get(Directory));
        try {
            createTarGzipFolder(prepared);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public void createTarGzipFolder(ArrayList<Path> source) throws IOException {
        String tarFileName = ArchivePath;

        try (OutputStream fOut = Files.newOutputStream(Paths.get(tarFileName));
             BufferedOutputStream buffOut = new BufferedOutputStream(fOut);
             GzipCompressorOutputStream gzOut = new GzipCompressorOutputStream(buffOut);
             TarArchiveOutputStream tOut = new TarArchiveOutputStream(gzOut)) {

            for(int i = 0; i < source.size(); ++i) {
                try {
                    TarArchiveEntry tarEntry = new TarArchiveEntry(source.get(i).toFile());

                    tOut.putArchiveEntry(tarEntry);

                    Files.copy(source.get(i), tOut);

                    tOut.closeArchiveEntry();
                } catch (IOException e) {
                    System.err.printf("Unable to tar.gz : %s%n%s%n", source.get(i), e);
                }
            }

            tOut.finish();
        }

    }

    private ArrayList<Path> GetRecurFiles(Path directory)
    {
        List<Path> files = new ArrayList<>();
        try {
            files = Files.walk(directory)
                    .filter(Files::isRegularFile)
                    .collect(Collectors.toList());
        } catch (IOException e) {
            e.printStackTrace();
        }

        List<Path> directories = new ArrayList<>();
        try {
            directories = Files.walk(directory)
                    .filter(Files::isDirectory)
                    .collect(Collectors.toList());
        } catch (IOException e) {
            e.printStackTrace();
        }

        var list = new ArrayList<Path>(files);
        var tmplist = new ArrayList<ArrayList<Path>>();
        for(int i = 0; i < list.size(); ++i) {
            tmplist.add(GetRecurFiles(list.get(i)));
        }
        for(int i = 0; i < tmplist.size(); ++i) {
            list.addAll(tmplist.get(i));
        }
        return list;
    }

    private ArrayList<Path> GetPreparedFiles(Path directory) throws Exception {
        var files = GetRecurFiles(directory);
        if (files.size() == 0)
        {
            throw new Exception("Directory is empty");
        }
        var prepared = new ArrayList<Path>();

        for(int i = 0; i < files.size(); ++i) {
            if(files.get(i).toString().endsWith(".c") || files.get(i).toString().endsWith(".h")) {
                prepared.add(files.get(i));
            }
        }

        if (prepared.size() == 0)
        {
            throw new Exception("Directory haven't .c or .h files");
        }
        return prepared;
    }

    public void Update() throws Exception {
        java.nio.file.Path apath = Paths.get(ArchivePath);
        if(!Files.exists(apath))
        {
            throw new Exception("Archive doesn't exist");
        }
        var tmp = Files.createDirectory(Paths.get(ArchivePath + "/tmp"));
        decompressTarGzipFile(apath, tmp);

        Files.createFile(Paths.get(tmp.toUri() + "/foo.c"));
        Files.createFile(Paths.get(tmp.toUri() + "/bar.c"));
        Files.delete(apath);

        var prepared = GetPreparedFiles(tmp);
        createTarGzipFolder(prepared);
        Files.delete(tmp);
    }

    public void Extract() throws IOException {
        java.nio.file.Path apath = Paths.get(ArchivePath);
        java.nio.file.Path opath = Paths.get(Output);
        decompressTarGzipFile(apath, opath);
    }

    public static void decompressTarGzipFile(Path source, Path target)
            throws IOException {

        if (Files.notExists(source)) {
            throw new IOException("File doesn't exists!");
        }

        try (InputStream fi = Files.newInputStream(source);
             BufferedInputStream bi = new BufferedInputStream(fi);
             GzipCompressorInputStream gzi = new GzipCompressorInputStream(bi);
             TarArchiveInputStream ti = new TarArchiveInputStream(gzi)) {

            ArchiveEntry entry;
            while ((entry = ti.getNextEntry()) != null) {

                Path newPath = zipSlipProtect(entry, target);

                if (entry.isDirectory()) {
                    Files.createDirectories(newPath);
                } else {

                    // check parent folder again
                    Path parent = newPath.getParent();
                    if (parent != null) {
                        if (Files.notExists(parent)) {
                            Files.createDirectories(parent);
                        }
                    }

                    // copy TarArchiveInputStream to Path newPath
                    Files.copy(ti, newPath, StandardCopyOption.REPLACE_EXISTING);

                }
            }
        }
    }

    private static Path zipSlipProtect(ArchiveEntry entry, Path targetDir)
            throws IOException {

        Path targetDirResolved = targetDir.resolve(entry.getName());

        Path normalizePath = targetDirResolved.normalize();

        if (!normalizePath.startsWith(targetDir)) {
            throw new IOException("Bad entry: " + entry.getName());
        }

        return normalizePath;
    }

    public String GetDirectory() {
        return Directory;
    }
    public String GetArchivePath() {
        return ArchivePath;
    }
    public boolean GetCanOverwrite() {
        return CanOverwrite;
    }
    public String GetOutput() {
        return Output;
    }
}
